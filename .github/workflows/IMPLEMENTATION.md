# Daily Template Size Tracking - Implementation Summary

## Overview

This implementation provides automated daily tracking of Uno Platform template package sizes across multiple platforms and .NET versions.

## Files Created

### Workflow
- `.github/workflows/daily-template-size-tracking.yml` - Main GitHub Actions workflow

### Scripts
- `.github/scripts/measure-package-size.ps1` - Measures package sizes and collects metrics
- `.github/scripts/upload-to-azure.ps1` - Uploads metrics to Azure Blob Storage
- `.github/scripts/compare-and-alert.ps1` - Compares metrics and generates alerts
- `.github/scripts/generate-summary.ps1` - Creates GitHub Actions summary reports

### Documentation
- `.github/workflows/README.md` - Complete documentation
- `.github/workflows/SETUP.md` - Quick start guide

## Key Features Implemented

### ✅ Template Installation
- Queries NuGet API for latest prerelease version of Uno.Templates
- Uses validated `dotnet new install Package::Version` syntax
- Supports manual version override via workflow dispatch

### ✅ Multi-Platform Support

| Platform | Build Type | Metric |
|----------|-----------|--------|
| Android | AAB (unsigned) | File size |
| iOS | IPA (signed device build) | File size |
| WebAssembly | Published wwwroot | Folder size |
| Windows Desktop | Self-contained (net-desktop) | Folder size |
| Linux Desktop | Self-contained (net-desktop) | Folder size |
| macOS Desktop | Self-contained (net-desktop) | Folder size |

### ✅ Multi-.NET Version Support
- .NET 9.0
- .NET 10.0
- Configurable via workflow dispatch

### ✅ Template Variations
- Blank preset
- Recommended preset
- Extensible to other presets

### ✅ Comprehensive Metrics
For each build:
- Package size (platform-specific)
- Compressed size (ZIP)
- Build time (seconds)
- File count
- Assembly count (.dll files)
- Total assembly size
- Largest file information
- Platform-specific metrics (e.g., WASM file sizes)

### ✅ Data Storage

- Azure Blob Storage with hierarchical structure
- Year-based organization: `YYYY/MM-DD-HH-MM/net{version}/`
- Latest reference copies for quick comparison
- JSON format for easy parsing

### ✅ Alerting System
- Configurable thresholds (default: 10% alert, 20% failure)
- Automatic GitHub issue creation on alerts
- Color-coded indicators in summary
- Trend analysis with previous day comparison

### ✅ Reporting
- GitHub Actions Summary with formatted tables
- Trend indicators (🔴🟡🟢 for increases/decreases)
- Top increases/decreases lists
- Build time tracking
- Links to detailed metrics in Azure

### ✅ Scheduling
- Daily at midnight ET (5 AM UTC)
- Manual trigger with custom parameters
- Configurable versions, templates, and thresholds

## Workflow Architecture

```
┌─────────────────────┐
│  prepare-matrix     │  - Install Uno.Templates
│                     │  - Generate build matrix
└──────────┬──────────┘
           │
           ├─────────────────────────────┐
           │                             │
┌──────────▼──────────┐      ┌──────────▼──────────┐
│ build-and-measure   │      │ build-and-measure   │
│  (matrix: 24 jobs)  │ ...  │  (matrix: 24 jobs)  │
│                     │      │                     │
│ - Create project    │      │ - Create project    │
│ - Build release     │      │ - Build release     │
│ - Measure size      │      │ - Measure size      │
│ - Upload metrics    │      │ - Upload metrics    │
└──────────┬──────────┘      └──────────┬──────────┘
           │                             │
           └─────────────┬───────────────┘
                         │
              ┌──────────▼──────────┐
              │ analyze-and-report  │
              │                     │
              │ - Upload to Azure   │
              │ - Download previous │
              │ - Compare & alert   │
              │ - Generate summary  │
              │ - Create issue      │
              └─────────────────────┘
```

## Matrix Strategy

The workflow uses a dynamic matrix with 24 combinations:

- 2 .NET versions (9.0, 10.0)
- 2 templates (blank, recommended)
- 6 platforms (android, ios, wasm, desktop-windows, desktop-linux, desktop-macos)

Total: 2 × 2 × 6 = **24 parallel jobs**

## Setup Required

### GitHub Secrets
Add to repository Settings → Secrets and variables → Actions:

1. `AZURE_STORAGE_ACCOUNT_NAME` (required)
2. `AZURE_STORAGE_ACCOUNT_KEY` (required)

### Azure Resources
- Azure Storage Account
- Container: `template-size-tracking` (auto-created by workflow)

### Permissions
GitHub Actions needs:
- `contents: read` (default)
- `issues: write` (for alert creation)

## Usage Examples

### Default Daily Run
Automatically runs at midnight ET with:
- Latest prerelease Uno.Templates
- .NET 9.0 and 10.0
- Blank and recommended templates
- All 6 platforms
- 10% alert threshold

### Manual Run - Test Specific Version
```yaml
Workflow dispatch inputs:
  uno_template_version: 5.5.0-dev.123
  dotnet_versions: 9.0
  templates: blank
  alert_threshold: 5
```

### Manual Run - Full Matrix Test
```yaml
Workflow dispatch inputs:
  uno_template_version: (leave empty)
  dotnet_versions: 9.0,10.0
  templates: blank,recommended
  alert_threshold: 10
```

## Data Access

### Via Azure Portal
1. Navigate to Storage Account
2. Select Containers → `template-size-tracking`
3. Browse by date path

### Via Azure CLI
```bash
# List today's metrics
az storage blob list \
  --account-name <account> \
  --container-name template-size-tracking \
  --prefix "$(date +%Y)/$(date +%m-%d-)"

# Download metric
az storage blob download \
  --account-name <account> \
  --container-name template-size-tracking \
  --name "2025/10-31-05-00/net9.0/blank-wasm.json" \
  --file metrics.json
```

### Via Storage Explorer
Use Azure Storage Explorer desktop app for GUI browsing.

## Extensibility

### Adding Platforms

Edit the matrix generation in `Prepare Build Matrix` step:

```powershell
$matrix.include += @{
    dotnet = $dotnet
    template = $template
    platform = 'new-platform'
    os = 'ubuntu-latest'
    framework = "net$dotnet-framework"
    rid = 'runtime-id'
}
```

### Adding Templates
Modify the `templates` input default or pass via workflow dispatch:
```yaml
templates: 'blank,recommended,default,custom'
```

### Adding Metrics
Extend `measure-package-size.ps1` to collect additional metrics:
```powershell
$metrics.customMetric = # your calculation
```

### Changing Alert Logic
Modify `compare-and-alert.ps1` thresholds or add new conditions.

## Maintenance

### Regular Tasks
- Review Azure Storage costs
- Clean up old metrics (>90 days)
- Update .NET versions as new releases come out
- Monitor alert frequency

### Troubleshooting
- Check workflow logs in Actions tab
- Verify Azure credentials in secrets
- Review individual job logs for build failures
- Test scripts locally before committing changes

## Performance Considerations

- **Parallel Execution**: 24 jobs run in parallel (subject to runner limits)
- **Build Time**: ~3-5 minutes per platform (varies)
- **Total Duration**: ~10-15 minutes for full matrix
- **Storage**: ~1-2 MB per day (all metrics combined)
- **Runner Costs**: Uses GitHub-hosted runners (free for public repos)

## Security

- **Signed iOS Builds**: Uses Apple Development certificate with automatic provisioning
- **Unsigned Android**: AAB files are unsigned (no keystore required)
- **Azure Credentials**: Stored as GitHub encrypted secrets
- **Template Safety**: Only official Uno.Templates from NuGet.org
- **No Secrets in Logs**: Azure keys are masked in outputs

## Future Enhancements

Potential improvements:
- Add build time benchmarks
- Include memory usage metrics
- Add app startup time measurements
- Create trend graphs/dashboards
- Integrate with Pull Request checks
- Add multi-architecture support (ARM, x86)
- Include detailed assembly analysis
- Add historical trend visualization

## Testing

Before first production run:
1. Test with manual workflow dispatch
2. Use specific Uno.Templates version
3. Test single platform first (e.g., WASM only)
4. Verify Azure uploads
5. Check GitHub Actions summary
6. Wait 24 hours and verify comparison works

## Support

For issues:
- Check `.github/workflows/README.md` for detailed docs
- Review `.github/workflows/SETUP.md` for setup guide
- Open GitHub issue for bugs or questions
- Check workflow run logs for errors

---

**Implementation Complete** ✅

All components are ready for deployment. Configure Azure Storage and GitHub Secrets to activate.
