#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Uploads metrics to Azure Blob Storage.

.PARAMETER MetricsPath
    Path to the directory containing metrics JSON files.
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$MetricsPath
)

$ErrorActionPreference = "Stop"

Write-Host "=== Uploading Metrics to Azure Storage ===" -ForegroundColor Cyan

$storageAccount = $env:AZURE_STORAGE_ACCOUNT
$storageKey = $env:AZURE_STORAGE_KEY

if ([string]::IsNullOrEmpty($storageAccount) -or [string]::IsNullOrEmpty($storageKey)) {
    Write-Error "Azure Storage credentials not found in environment variables"
    exit 1
}

$containerName = "template-size-tracking"
$date = Get-Date
$dateTimePath = $date.ToString("MM-dd-HH-mm")
$year = $date.ToString("yyyy")

Write-Host "Storage Account: $storageAccount"
Write-Host "Container: $containerName"
Write-Host "Year: $year"
Write-Host "Date-Time Path: $dateTimePath"

# Ensure container exists
Write-Host "Ensuring container exists..." -ForegroundColor Yellow
az storage container create `
    --name $containerName `
    --account-name $storageAccount `
    --account-key $storageKey `
    --output none 2>$null

# Upload all metrics files
Write-Host "Uploading metrics files..." -ForegroundColor Yellow

$metricsFiles = Get-ChildItem -Path $MetricsPath -Filter "*.json" -Recurse

foreach ($file in $metricsFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Gray
    
    # Parse the file to get metadata
    $content = Get-Content $file.FullName -Raw | ConvertFrom-Json
    
    $dotnetVersion = $content.dotnetVersion
    $template = $content.template
    $platform = $content.platform
    
    # Upload to year/date-time/tfm/platform structure
    $blobPath = "$year/$dateTimePath/net$dotnetVersion/$template-$platform.json"
    
    Write-Host "  -> Uploading to: $blobPath"
    
    az storage blob upload `
        --account-name $storageAccount `
        --account-key $storageKey `
        --container-name $containerName `
        --name $blobPath `
        --file $file.FullName `
        --overwrite `
        --output none
    
    # Also upload to "latest" for quick reference
    $latestPath = "latest/net$dotnetVersion/$template-$platform.json"
    
    az storage blob upload `
        --account-name $storageAccount `
        --account-key $storageKey `
        --container-name $containerName `
        --name $latestPath `
        --file $file.FullName `
        --overwrite `
        --output none
    
    Write-Host "  ✓ Uploaded successfully" -ForegroundColor Green
}

Write-Host "`n=== Upload Complete ===" -ForegroundColor Green
Write-Host "Total files uploaded: $($metricsFiles.Count)"
Write-Host "Azure Storage URL: https://$storageAccount.blob.core.windows.net/$containerName/$year/$dateTimePath/"
