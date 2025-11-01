#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Measures package size and collects metrics for Uno Platform templates.

.PARAMETER PublishPath
    Path to the publish output directory.

.PARAMETER Platform
    Target platform (android, ios, wasm, windows, desktop, macos).

.PARAMETER Template
    Template type (blank, recommended).

.PARAMETER DotNetVersion
    .NET version used for the build.

.PARAMETER UnoVersion
    Uno.Templates version used.

.PARAMETER BuildTime
    Build time in seconds.

.PARAMETER OutputPath
    Path where the metrics JSON file will be saved.
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$PublishPath,
    
    [Parameter(Mandatory=$true)]
    [string]$Platform,
    
    [Parameter(Mandatory=$true)]
    [string]$Template,
    
    [Parameter(Mandatory=$true)]
    [string]$DotNetVersion,
    
    [Parameter(Mandatory=$true)]
    [string]$UnoVersion,
    
    [Parameter(Mandatory=$true)]
    [decimal]$BuildTime,
    
    [Parameter(Mandatory=$true)]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

Write-Host "=== Measuring Package Size ===" -ForegroundColor Cyan
Write-Host "Platform: $Platform"
Write-Host "Template: $Template"
Write-Host "Publish Path: $PublishPath"

# Initialize metrics object
$metrics = @{
    timestamp = (Get-Date).ToUniversalTime().ToString("o")
    template = $Template
    platform = $Platform
    dotnetVersion = $DotNetVersion
    unoVersion = $UnoVersion
    buildTimeSeconds = [math]::Round($BuildTime, 2)
}

# Function to get directory size
function Get-DirectorySize {
    param([string]$Path)
    
    if (-not (Test-Path $Path)) {
        return 0
    }
    
    $size = (Get-ChildItem -Path $Path -Recurse -File -ErrorAction SilentlyContinue | 
        Measure-Object -Property Length -Sum).Sum
    
    if ($null -eq $size) {
        return 0
    }
    
    return $size
}

# Function to count files
function Get-FileCount {
    param([string]$Path, [string]$Filter = "*")
    
    if (-not (Test-Path $Path)) {
        return 0
    }
    
    return (Get-ChildItem -Path $Path -Filter $Filter -Recurse -File -ErrorAction SilentlyContinue | 
        Measure-Object).Count
}

# Function to get largest file
function Get-LargestFile {
    param([string]$Path)
    
    if (-not (Test-Path $Path)) {
        return $null
    }
    
    $largest = Get-ChildItem -Path $Path -Recurse -File -ErrorAction SilentlyContinue | 
        Sort-Object Length -Descending | 
        Select-Object -First 1
    
    if ($largest) {
        return @{
            name = $largest.Name
            size = $largest.Length
        }
    }
    
    return $null
}

# Platform-specific measurements
switch ($Platform) {
    "android" {
        Write-Host "Measuring Android AAB package..." -ForegroundColor Yellow
        
        $aabFile = Get-ChildItem -Path $PublishPath -Filter "*.aab" -Recurse | Select-Object -First 1
        
        if ($aabFile) {
            $metrics.packageSize = $aabFile.Length
            $metrics.packagePath = $aabFile.Name
            Write-Host "AAB Size: $($aabFile.Length / 1MB) MB"
        } else {
            Write-Warning "No AAB file found"
            $metrics.packageSize = 0
        }
        
        # Additional metrics
        $metrics.totalPublishSize = Get-DirectorySize -Path $PublishPath
        $metrics.fileCount = Get-FileCount -Path $PublishPath
        $metrics.assemblyCount = Get-FileCount -Path $PublishPath -Filter "*.dll"
    }
    
    "ios" {
        Write-Host "Measuring iOS IPA package..." -ForegroundColor Yellow
        
        $ipaFile = Get-ChildItem -Path $PublishPath -Filter "*.ipa" -Recurse | Select-Object -First 1
        
        if ($ipaFile) {
            $metrics.packageSize = $ipaFile.Length
            $metrics.packagePath = $ipaFile.Name
            Write-Host "IPA Size: $($ipaFile.Length / 1MB) MB"
        } else {
            Write-Warning "No IPA file found, measuring publish directory"
            $metrics.packageSize = Get-DirectorySize -Path $PublishPath
        }
        
        $metrics.totalPublishSize = Get-DirectorySize -Path $PublishPath
        $metrics.fileCount = Get-FileCount -Path $PublishPath
        $metrics.assemblyCount = Get-FileCount -Path $PublishPath -Filter "*.dll"
    }
    
    "wasm" {
        Write-Host "Measuring WebAssembly wwwroot..." -ForegroundColor Yellow
        
        $wwwrootPath = Get-ChildItem -Path $PublishPath -Filter "wwwroot" -Recurse -Directory | 
            Select-Object -First 1
        
        if ($wwwrootPath) {
            $metrics.packageSize = Get-DirectorySize -Path $wwwrootPath.FullName
            $metrics.packagePath = "wwwroot"
            Write-Host "wwwroot Size: $($metrics.packageSize / 1MB) MB"
            
            # WASM-specific metrics
            $metrics.wasmFileSize = (Get-ChildItem -Path $wwwrootPath.FullName -Filter "*.wasm" -Recurse | 
                Measure-Object -Property Length -Sum).Sum
            $metrics.jsFileSize = (Get-ChildItem -Path $wwwrootPath.FullName -Filter "*.js" -Recurse | 
                Measure-Object -Property Length -Sum).Sum
            $metrics.dllFileSize = (Get-ChildItem -Path $wwwrootPath.FullName -Filter "*.dll" -Recurse | 
                Measure-Object -Property Length -Sum).Sum
        } else {
            $metrics.packageSize = Get-DirectorySize -Path $PublishPath
        }
        
        $metrics.totalPublishSize = Get-DirectorySize -Path $PublishPath
        $metrics.fileCount = Get-FileCount -Path $PublishPath
        $metrics.assemblyCount = Get-FileCount -Path $PublishPath -Filter "*.dll"
    }
    
    { $_ -in "windows", "desktop", "macos" } {
        Write-Host "Measuring Desktop publish folder..." -ForegroundColor Yellow
        
        $metrics.packageSize = Get-DirectorySize -Path $PublishPath
        $metrics.packagePath = "publish"
        Write-Host "Publish Folder Size: $($metrics.packageSize / 1MB) MB"
        
        # Desktop-specific metrics
        $metrics.totalPublishSize = $metrics.packageSize
        $metrics.fileCount = Get-FileCount -Path $PublishPath
        $metrics.assemblyCount = Get-FileCount -Path $PublishPath -Filter "*.dll"
        
        # Find main executable
        $exePattern = if ($Platform -eq "windows") { "*.exe" } else { "*" }
        $mainExe = Get-ChildItem -Path $PublishPath -Filter $exePattern -File | 
            Where-Object { $_.Length -gt 1MB } | 
            Sort-Object Length -Descending | 
            Select-Object -First 1
        
        if ($mainExe) {
            $metrics.mainExecutableSize = $mainExe.Length
            $metrics.mainExecutableName = $mainExe.Name
        }
    }
}

# Common metrics for all platforms
$metrics.totalAssemblySize = (Get-ChildItem -Path $PublishPath -Filter "*.dll" -Recurse -ErrorAction SilentlyContinue | 
    Measure-Object -Property Length -Sum).Sum

# Get largest file info
$largestFile = Get-LargestFile -Path $PublishPath
if ($largestFile) {
    $metrics.largestFile = $largestFile
}

# Create compressed archive for comparison
$tempZip = Join-Path ([System.IO.Path]::GetTempPath()) "package.zip"
if (Test-Path $tempZip) {
    Remove-Item $tempZip -Force
}

Write-Host "Creating compressed archive for measurement..." -ForegroundColor Yellow
try {
    Compress-Archive -Path "$PublishPath\*" -DestinationPath $tempZip -CompressionLevel Optimal -Force
    $metrics.compressedSize = (Get-Item $tempZip).Length
    Write-Host "Compressed Size: $($metrics.compressedSize / 1MB) MB"
    Remove-Item $tempZip -Force
} catch {
    Write-Warning "Could not create compressed archive: $_"
    $metrics.compressedSize = 0
}

# Calculate compression ratio
if ($metrics.compressedSize -gt 0 -and $metrics.packageSize -gt 0) {
    $metrics.compressionRatio = [math]::Round(($metrics.compressedSize / $metrics.packageSize) * 100, 2)
}

# SDK version info
try {
    $sdkVersion = dotnet --version
    $metrics.dotnetSdkVersion = $sdkVersion.Trim()
} catch {
    $metrics.dotnetSdkVersion = "unknown"
}

# Output summary
Write-Host "`n=== Metrics Summary ===" -ForegroundColor Green
Write-Host "Package Size: $([math]::Round($metrics.packageSize / 1MB, 2)) MB"
Write-Host "Compressed Size: $([math]::Round($metrics.compressedSize / 1MB, 2)) MB"
Write-Host "File Count: $($metrics.fileCount)"
Write-Host "Assembly Count: $($metrics.assemblyCount)"
Write-Host "Build Time: $($metrics.buildTimeSeconds) seconds"

# Save metrics to JSON
$metricsJson = $metrics | ConvertTo-Json -Depth 10
$metricsJson | Out-File -FilePath $OutputPath -Encoding UTF8

Write-Host "`nMetrics saved to: $OutputPath" -ForegroundColor Green
