#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Compares current metrics with previous day and generates alerts.

.PARAMETER CurrentMetricsPath
    Path to current metrics directory.

.PARAMETER PreviousMetricsPath
    Path to previous metrics directory.

.PARAMETER AlertThreshold
    Percentage threshold for alerts (default: 10).

.PARAMETER FailureThreshold
    Percentage threshold for critical failures (default: 20).
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$CurrentMetricsPath,
    
    [Parameter(Mandatory=$true)]
    [string]$PreviousMetricsPath,
    
    [Parameter(Mandatory=$false)]
    [decimal]$AlertThreshold = 10,
    
    [Parameter(Mandatory=$false)]
    [decimal]$FailureThreshold = 20
)

$ErrorActionPreference = "Stop"

Write-Host "=== Comparing Metrics ===" -ForegroundColor Cyan
Write-Host "Alert Threshold: $AlertThreshold%"
Write-Host "Failure Threshold: $FailureThreshold%"

# Load current metrics
$currentMetrics = @()
$currentFiles = Get-ChildItem -Path $CurrentMetricsPath -Filter "*.json" -Recurse

foreach ($file in $currentFiles) {
    $content = Get-Content $file.FullName -Raw | ConvertFrom-Json
    $currentMetrics += $content
}

Write-Host "Loaded $($currentMetrics.Count) current metrics"

# Load previous metrics if they exist
$previousMetrics = @()
if (Test-Path $PreviousMetricsPath) {
    $previousFiles = Get-ChildItem -Path $PreviousMetricsPath -Filter "*.json" -Recurse
    
    foreach ($file in $previousFiles) {
        try {
            $content = Get-Content $file.FullName -Raw | ConvertFrom-Json
            $previousMetrics += $content
        } catch {
            Write-Warning "Could not parse previous metric file: $($file.Name)"
        }
    }
    
    Write-Host "Loaded $($previousMetrics.Count) previous metrics"
} else {
    Write-Host "No previous metrics found - this appears to be the first run"
}

# Compare metrics
$comparisons = @()
$alerts = @()
$criticalAlerts = @()

foreach ($current in $currentMetrics) {
    # Find matching previous metric
    $previous = $previousMetrics | Where-Object {
        $_.dotnetVersion -eq $current.dotnetVersion -and
        $_.template -eq $current.template -and
        $_.platform -eq $current.platform
    } | Select-Object -First 1
    
    $comparison = @{
        dotnetVersion = $current.dotnetVersion
        template = $current.template
        platform = $current.platform
        currentSize = $current.packageSize
        currentBuildTime = $current.buildTimeSeconds
        previousSize = if ($previous) { $previous.packageSize } else { 0 }
        previousBuildTime = if ($previous) { $previous.buildTimeSeconds } else { 0 }
        sizeChange = 0
        sizeChangePercent = 0
        buildTimeChange = 0
        buildTimeChangePercent = 0
        status = "new"
    }
    
    if ($previous) {
        if ($previous.packageSize -gt 0) {
            $comparison.sizeChange = $current.packageSize - $previous.packageSize
            $comparison.sizeChangePercent = [math]::Round(($comparison.sizeChange / $previous.packageSize) * 100, 2)
        }
        
        if ($previous.buildTimeSeconds -gt 0) {
            $comparison.buildTimeChange = $current.buildTimeSeconds - $previous.buildTimeSeconds
            $comparison.buildTimeChangePercent = [math]::Round(($comparison.buildTimeChange / $previous.buildTimeSeconds) * 100, 2)
        }
        
        # Determine status
        if ($comparison.sizeChangePercent -ge $FailureThreshold) {
            $comparison.status = "critical"
            $criticalAlerts += $comparison
        } elseif ($comparison.sizeChangePercent -ge $AlertThreshold) {
            $comparison.status = "alert"
            $alerts += $comparison
        } elseif ($comparison.sizeChangePercent -gt 0) {
            $comparison.status = "increased"
        } elseif ($comparison.sizeChangePercent -lt 0) {
            $comparison.status = "decreased"
        } else {
            $comparison.status = "unchanged"
        }
    }
    
    $comparisons += $comparison
}

# Save comparison results
$comparisons | ConvertTo-Json -Depth 10 | Out-File -FilePath "comparison.json" -Encoding UTF8

# Generate alert report if needed
if ($alerts.Count -gt 0 -or $criticalAlerts.Count -gt 0) {
    Write-Host "`n⚠️  ALERTS DETECTED!" -ForegroundColor Red
    
    $alertReport = @"
# ⚠️ Template Size Alert - $(Get-Date -Format "yyyy-MM-dd")

Significant size increases detected in Uno Platform templates.

"@
    
    if ($criticalAlerts.Count -gt 0) {
        $alertReport += @"

## 🔴 Critical Increases (>$FailureThreshold%)

| Template | Platform | .NET | Previous | Current | Change |
|----------|----------|------|----------|---------|--------|

"@
        
        foreach ($alert in $criticalAlerts) {
            $prevMB = [math]::Round($alert.previousSize / 1MB, 2)
            $currMB = [math]::Round($alert.currentSize / 1MB, 2)
            $change = "+$($alert.sizeChangePercent)%"
            
            $alertReport += "| $($alert.template) | $($alert.platform) | $($alert.dotnetVersion) | $prevMB MB | $currMB MB | $change 🔴 |`n"
        }
    }
    
    if ($alerts.Count -gt 0) {
        $alertReport += @"

## 🟡 Notable Increases (>$AlertThreshold%)

| Template | Platform | .NET | Previous | Current | Change |
|----------|----------|------|----------|---------|--------|

"@
        
        foreach ($alert in $alerts) {
            $prevMB = [math]::Round($alert.previousSize / 1MB, 2)
            $currMB = [math]::Round($alert.currentSize / 1MB, 2)
            $change = "+$($alert.sizeChangePercent)%"
            
            $alertReport += "| $($alert.template) | $($alert.platform) | $($alert.dotnetVersion) | $prevMB MB | $currMB MB | $change 🟡 |`n"
        }
    }
    
    $storageAccount = $env:AZURE_STORAGE_ACCOUNT
    $date = Get-Date
    $year = $date.ToString("yyyy")
    $dateTime = $date.ToString("MM-dd-HH-mm")
    
    $alertReport += @"

## Details

- **Workflow Run**: [View Details]($($env:GITHUB_SERVER_URL)/$($env:GITHUB_REPOSITORY)/actions/runs/$($env:GITHUB_RUN_ID))
- **Azure Storage**: [View Metrics](https://${storageAccount}.blob.core.windows.net/template-size-tracking/$year/$dateTime/)
- **Alert Threshold**: $AlertThreshold%
- **Failure Threshold**: $FailureThreshold%

---
*This issue was automatically generated by the daily template size tracking workflow.*
"@
    
    $alertReport | Out-File -FilePath "alert-report.md" -Encoding UTF8
    
    # Set output for GitHub Actions
    Add-Content -Path $env:GITHUB_OUTPUT -Value "alert=true"
    
    if ($criticalAlerts.Count -gt 0) {
        Add-Content -Path $env:GITHUB_OUTPUT -Value "critical=true"
        Write-Host "Critical alerts: $($criticalAlerts.Count)" -ForegroundColor Red
    } else {
        Add-Content -Path $env:GITHUB_OUTPUT -Value "critical=false"
    }
} else {
    Write-Host "`n✓ No significant size increases detected" -ForegroundColor Green
    Add-Content -Path $env:GITHUB_OUTPUT -Value "alert=false"
    Add-Content -Path $env:GITHUB_OUTPUT -Value "critical=false"
}

# Output summary
Write-Host "`n=== Comparison Summary ===" -ForegroundColor Cyan
Write-Host "Total comparisons: $($comparisons.Count)"
Write-Host "New entries: $(($comparisons | Where-Object { $_.status -eq 'new' }).Count)"
Write-Host "Decreased: $(($comparisons | Where-Object { $_.status -eq 'decreased' }).Count)"
Write-Host "Unchanged: $(($comparisons | Where-Object { $_.status -eq 'unchanged' }).Count)"
Write-Host "Increased: $(($comparisons | Where-Object { $_.status -eq 'increased' }).Count)"
Write-Host "Alerts: $($alerts.Count)" -ForegroundColor Yellow
Write-Host "Critical: $($criticalAlerts.Count)" -ForegroundColor Red
