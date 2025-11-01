# Daily Template Size Tracking

This workflow automatically tracks the package sizes of Uno Platform templates across different platforms and .NET versions.

## Features

- **Multi-Platform Support**: Android, iOS, WebAssembly, Windows, Linux, macOS
- **Multiple .NET Versions**: Tracks .NET 9.0 and 10.0
- **Template Variations**: Tracks both `blank` and `recommended` presets
- **Single-Project Structure**: Supports Uno Platform 5.2+ single-project template with Uno.Sdk
- **Comprehensive Metrics**: Package size, compressed size, file counts, assembly counts, build times
- **Historical Tracking**: Stores results in Azure Blob Storage
- **Trend Analysis**: Compares with previous day and detects significant changes
- **Automated Alerts**: Creates GitHub issues when size increases exceed threshold

## Uno Platform Project Structure

This workflow is designed for **Uno Platform 5.2+ templates** using the **single-project structure**:

- **Single .csproj file**: One project file targets all platforms
- **Platforms folder**: Contains platform-specific code:
  - `Platforms/Android/` - Android-specific code
  - `Platforms/iOS/` - iOS-specific code
  - `Platforms/WebAssembly/` - WebAssembly-specific code
  - `Platforms/Desktop/` - Desktop-specific code (Windows/Linux/macOS)
- **Framework targeting**: Each platform uses a specific target framework:
  - Android: `net{version}-android` (e.g., `net9.0-android`)
  - iOS: `net{version}-ios`
  - WebAssembly: `net{version}-browserwasm`
  - Desktop: `net{version}-desktop` (shared for Windows, Linux, macOS)

The build process finds the single `.csproj` file and uses the `-f` parameter to specify the target framework for each platform.

## Setup

### Prerequisites

1. **Azure Storage Account**
   - Create an Azure Storage Account
   - Note the account name and access key

2. **GitHub Secrets**
   
   Add the following secrets to your repository:
   
   - `AZURE_STORAGE_ACCOUNT_NAME`: Your Azure Storage account name
   - `AZURE_STORAGE_ACCOUNT_KEY`: Your Azure Storage account key

   Optional secrets:
   - `ALERT_THRESHOLD_PERCENT`: Percentage threshold for alerts (default: 10)
   - `FAILURE_THRESHOLD_PERCENT`: Percentage threshold for critical failures (default: 20)

### Configuration

The workflow can be configured via workflow dispatch inputs:

- **uno_template_version**: Specify a particular version of Uno.Templates (leave empty for latest prerelease)
- **dotnet_versions**: Comma-separated .NET versions to test (default: "9.0,10.0")
- **templates**: Comma-separated templates to test (default: "blank,recommended")
- **alert_threshold**: Alert threshold percentage (default: "10")

## Usage

### Automatic Runs

The workflow runs automatically every day at midnight ET (5 AM UTC).

### Manual Runs

1. Go to **Actions** → **Daily Template Size Tracking**
2. Click **Run workflow**
3. Optionally specify:
   - Specific Uno.Templates version
   - Specific .NET versions
   - Specific templates
   - Custom alert threshold

### Example: Testing a Specific Version

```yaml
Inputs:
  uno_template_version: 5.5.0-dev.123
  dotnet_versions: 9.0
  templates: blank
  alert_threshold: 5
```

## Metrics Collected

For each template/platform/version combination:

- **Package Size**:
  - AAB file size (Android)
  - IPA file size (iOS, signed device build)
  - wwwroot folder size (WebAssembly)
  - Publish folder size (Desktop platforms)
- **Compressed Size**: ZIP archive size for comparison
- **Build Time**: Total time to build the release package
- **File Count**: Total number of files in output
- **Assembly Count**: Number of .dll files
- **Total Assembly Size**: Combined size of all assemblies
- **Largest File**: Information about the largest file in output
- **Platform-Specific Metrics**:
  - WASM: Separate tracking for .wasm, .js, and .dll files
  - Desktop: Main executable size

## Data Storage

Metrics are stored in Azure Blob Storage with the following structure:

```
template-size-tracking/
  2025/
    10-31-05-00/
      net9.0/
        blank-android.json
        blank-ios.json
        blank-wasm.json
        blank-desktop-windows.json
        blank-desktop-linux.json
        blank-desktop-macos.json
        recommended-android.json
        ...
      net10.0/
        blank-android.json
        ...
    10-31-17-30/
      net9.0/
        ...
  2026/
    ...
  latest/
    net9.0/
      blank-android.json
      ...
    net10.0/
      ...
```

## Alerts

The workflow automatically creates GitHub issues when:

- Any template/platform combination shows **>10%** size increase (configurable)
- Issues are labeled with `size-alert` and `automated`
- Issues include:
  - Table of size increases
  - Links to detailed metrics
  - Links to workflow run

The workflow fails when:

- Any template/platform combination shows **>20%** size increase (configurable)

## Reports

Each run generates:

1. **GitHub Actions Summary**: 
   - Overview of all metrics
   - Trend analysis with color coding
   - Top increases/decreases
   
2. **Azure Storage Metrics**: 
   - Detailed JSON files for each configuration
   - Historical data for trending

3. **GitHub Issues** (when thresholds exceeded):
   - Automatic issue creation
   - Detailed comparison tables

## Workflow Structure

```
prepare-matrix
  ↓
  ├─ build-and-measure (matrix: 24 jobs)
  │  ├─ Create project with dotnet new unoapp
  │  ├─ Build for target platform
  │  ├─ Measure package size
  │  └─ Upload metrics
  ↓
analyze-and-report
  ├─ Upload to Azure Storage
  ├─ Download previous day's results
  ├─ Compare and detect anomalies
  ├─ Generate summary report
  └─ Create issue if thresholds exceeded
```

## Platforms and Frameworks

| Platform | OS | Framework Pattern | Runtime ID | Notes |
|----------|----|--------------------|------------|-------|
| Android | ubuntu-latest | net{version}-android | android-arm64 | AAB format |
| iOS | macos-latest | net{version}-ios | ios-arm64 | Signed device build |
| WebAssembly | ubuntu-latest | net{version}-browserwasm | browser-wasm | wwwroot folder |
| Windows Desktop | windows-latest | net{version}-desktop | win-x64 | Self-contained |
| Linux Desktop | ubuntu-latest | net{version}-desktop | linux-x64 | Self-contained, Skia rendering |
| macOS Desktop | macos-latest | net{version}-desktop | osx-x64 | Self-contained, Skia rendering |

## Troubleshooting

### No metrics uploaded

Check:
- Azure Storage credentials in secrets
- Azure CLI installation in workflow
- Network connectivity from GitHub Actions

### Build failures

- Check .NET SDK versions are available
- Verify workload installations
- Review build logs for specific platform errors

### No previous metrics for comparison

This is normal on the first run. Comparisons will be available after the second run.

## Customization

### Adding More Platforms

Edit `.github/workflows/daily-template-size-tracking.yml` and add entries to the matrix in the `Prepare Build Matrix` step.

### Adding More Templates

Add template names to the workflow dispatch input or modify the default in the workflow file.

### Changing Alert Thresholds

Modify the `ALERT_THRESHOLD` and `FAILURE_THRESHOLD` environment variables in the workflow file, or set them as secrets.

## Contributing

To test changes locally:

1. Run the measurement script:
   ```powershell
   .\.github\scripts\measure-package-size.ps1 `
     -PublishPath "path/to/publish" `
     -Platform "wasm" `
     -Template "blank" `
     -DotNetVersion "9.0" `
     -UnoVersion "5.5.0" `
     -BuildTime 120 `
     -OutputPath "metrics.json"
   ```

2. Run the comparison script:
   ```powershell
   .\.github\scripts\compare-and-alert.ps1 `
     -CurrentMetricsPath "current" `
     -PreviousMetricsPath "previous" `
     -AlertThreshold 10 `
     -FailureThreshold 20
   ```

## License

Same as the repository license.
