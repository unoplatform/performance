#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates a GitHub Actions summary report.

.PARAMETER MetricsPath
    Path to current metrics directory.

.PARAMETER ComparisonPath
    Path to comparison JSON file.
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$MetricsPath,
    
    [Parameter(Mandatory=$false)]
    [string]$ComparisonPath = "comparison.json"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Generating Summary Report ===" -ForegroundColor Cyan

# Load comparisons if available
$comparisons = @()
if (Test-Path $ComparisonPath) {
    $comparisons = Get-Content $ComparisonPath -Raw | ConvertFrom-Json
}

# Load current metrics
$currentMetrics = @()
$currentFiles = Get-ChildItem -Path $MetricsPath -Filter "*.json" -Recurse

foreach ($file in $currentFiles) {
    $content = Get-Content $file.FullName -Raw | ConvertFrom-Json
    $currentMetrics += $content
}

# Group by .NET version
$groupedByDotNet = $currentMetrics | Group-Object -Property dotnetVersion

# Start building the summary
$summary = @"
# 📊 Template Size Tracking - $(Get-Date -Format "yyyy-MM-dd")

"@

# Add overall statistics
$totalSize = ($currentMetrics | Measure-Object -Property packageSize -Sum).Sum
$avgBuildTime = ($currentMetrics | Measure-Object -Property buildTimeSeconds -Average).Average

$summary += @"

## Overall Statistics

- **Total Configurations**: $($currentMetrics.Count)
- **Total Package Size**: $([math]::Round($totalSize / 1GB, 2)) GB
- **Average Build Time**: $([math]::Round($avgBuildTime, 2)) seconds
- **Uno Templates Version**: $($currentMetrics[0].unoVersion)

"@

# Add tables for each .NET version
foreach ($dotnetGroup in $groupedByDotNet) {
    $dotnetVersion = $dotnetGroup.Name
    $summary += @"

## .NET $dotnetVersion

| Template | Platform | Size | Compressed | Files | Assemblies | Build Time | Change |
|----------|----------|------|------------|-------|------------|------------|--------|

"@
    
    foreach ($metric in $dotnetGroup.Group | Sort-Object template, platform) {
        # Find comparison
        $comparison = $comparisons | Where-Object {
            $_.dotnetVersion -eq $metric.dotnetVersion -and
            $_.template -eq $metric.template -and
            $_.platform -eq $metric.platform
        } | Select-Object -First 1
        
        $sizeMB = [math]::Round($metric.packageSize / 1MB, 2)
        $compressedMB = [math]::Round($metric.compressedSize / 1MB, 2)
        if ($metric.PSObject.Properties["buildTimeFormatted"] -and $metric.buildTimeFormatted) {
            $buildTime = $metric.buildTimeFormatted
        } else {
            $buildTime = "$([math]::Round($metric.buildTimeSeconds / 60, 1))m"
        }
        
        $changeIndicator = ""
        if ($comparison -and $comparison.status -ne "new") {
            $changePercent = $comparison.sizeChangePercent
            
            if ($changePercent -ge 20) {
                $changeIndicator = "+$changePercent% 🔴"
            } elseif ($changePercent -ge 10) {
                $changeIndicator = "+$changePercent% 🟡"
            } elseif ($changePercent -ge 5) {
                $changeIndicator = "+$changePercent% 🟠"
            } elseif ($changePercent -gt 0) {
                $changeIndicator = "+$changePercent% ↗️"
            } elseif ($changePercent -lt 0) {
                $changeIndicator = "$changePercent% 🟢"
            } else {
                $changeIndicator = "—"
            }
        } elseif ($comparison -and $comparison.status -eq "new") {
            $changeIndicator = "NEW ✨"
        } else {
            $changeIndicator = "—"
        }
        
        $summary += "| $($metric.template) | $($metric.platform) | $sizeMB MB | $compressedMB MB | $($metric.fileCount) | $($metric.assemblyCount) | $buildTime | $changeIndicator |`n"
    }
}

# Add trend analysis if comparisons available
if ($comparisons.Count -gt 0) {
    $increased = ($comparisons | Where-Object { $_.sizeChangePercent -gt 0 }).Count
    $decreased = ($comparisons | Where-Object { $_.sizeChangePercent -lt 0 }).Count
    $unchanged = ($comparisons | Where-Object { $_.sizeChangePercent -eq 0 }).Count
    $new = ($comparisons | Where-Object { $_.status -eq "new" }).Count
    
    $summary += @"

## Trend Analysis

- 📈 Increased: $increased
- 📉 Decreased: $decreased
- ➡️ Unchanged: $unchanged
- ✨ New: $new

"@
    
    # List top increases
    $topIncreases = $comparisons | 
        Where-Object { $_.sizeChangePercent -gt 0 } | 
        Sort-Object -Property sizeChangePercent -Descending | 
        Select-Object -First 5
    
    if ($topIncreases.Count -gt 0) {
        $summary += @"

### Top Size Increases

| Template | Platform | .NET | Change |
|----------|----------|------|--------|

"@
        
        foreach ($increase in $topIncreases) {
            $summary += "| $($increase.template) | $($increase.platform) | $($increase.dotnetVersion) | +$($increase.sizeChangePercent)% |`n"
        }
    }
    
    # List top decreases
    $topDecreases = $comparisons | 
        Where-Object { $_.sizeChangePercent -lt 0 } | 
        Sort-Object -Property sizeChangePercent | 
        Select-Object -First 5
    
    if ($topDecreases.Count -gt 0) {
        $summary += @"

### Top Size Decreases

| Template | Platform | .NET | Change |
|----------|----------|------|--------|

"@
        
        foreach ($decrease in $topDecreases) {
            $summary += "| $($decrease.template) | $($decrease.platform) | $($decrease.dotnetVersion) | $($decrease.sizeChangePercent)% |`n"
        }
    }
}

# Add links
$storageAccount = $env:AZURE_STORAGE_ACCOUNT
$date = Get-Date
$year = $date.ToString("yyyy")
$dateTime = $date.ToString("MM-dd-HH-mm")

$summary += @"

## Links

- 📦 [Azure Storage Metrics](https://${storageAccount}.blob.core.windows.net/template-size-tracking/$year/$dateTime/)
- 🔗 [Workflow Run]($($env:GITHUB_SERVER_URL)/$($env:GITHUB_REPOSITORY)/actions/runs/$($env:GITHUB_RUN_ID))
- 📚 [Repository]($($env:GITHUB_SERVER_URL)/$($env:GITHUB_REPOSITORY))

---
*Generated at $(Get-Date -Format "yyyy-MM-dd HH:mm:ss") UTC*
"@

# Write to GitHub Actions summary
$summaryFile = $env:GITHUB_STEP_SUMMARY
if ($summaryFile) {
    $summary | Out-File -FilePath $summaryFile -Encoding UTF8 -Append
    Write-Host "Summary written to GitHub Actions" -ForegroundColor Green
} else {
    Write-Host $summary
}

# Also save to file
$summary | Out-File -FilePath "summary-report.md" -Encoding UTF8
Write-Host "Summary saved to summary-report.md" -ForegroundColor Green
