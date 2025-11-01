# Template Size Tracking - Quick Start Guide

## Azure Storage Setup

### 1. Create Azure Storage Account

```bash
# Using Azure CLI
az storage account create \
  --name unotemplatemetrics \
  --resource-group uno-performance \
  --location eastus \
  --sku Standard_LRS

# Get the account key
az storage account keys list \
  --account-name unotemplatemetrics \
  --resource-group uno-performance \
  --query "[0].value" -o tsv
```

### 2. Configure GitHub Secrets

Go to your repository → Settings → Secrets and variables → Actions → New repository secret

Add these secrets:

| Secret Name | Value |
|-------------|-------|
| `AZURE_STORAGE_ACCOUNT_NAME` | Your storage account name (e.g., `unotemplatemetrics`) |
| `AZURE_STORAGE_ACCOUNT_KEY` | The access key from step 1 |

### 3. Test the Workflow

1. Go to **Actions** tab
2. Select **Daily Template Size Tracking**
3. Click **Run workflow**
4. Use default values for the first test
5. Monitor the workflow execution

## Viewing Results

### In GitHub Actions

After the workflow completes, view the summary:
1. Go to **Actions** → **Daily Template Size Tracking**
2. Click on the latest run
3. Scroll down to see the summary with tables and trends

### In Azure Storage

Browse metrics in Azure Portal or using Azure Storage Explorer:

```bash
# List today's metrics
az storage blob list \
  --account-name unotemplatemetrics \
  --container-name template-size-tracking \
  --prefix "$(date +%Y)/$(date +%m-%d-)" \
  --output table

# Download a specific metric
az storage blob download \
  --account-name unotemplatemetrics \
  --container-name template-size-tracking \
  --name "2025/10-31-05-00/net9.0/blank-wasm.json" \
  --file "metrics.json"
```

## Manual Testing

### Test Locally

1. Install Uno.Templates:
   ```bash
   dotnet new install Uno.Templates
   ```

2. Create a test project:
   ```bash
   dotnet new unoapp -o TestApp -preset blank
   ```

3. Build for a platform:
   ```bash
   # WebAssembly
   cd TestApp
   dotnet publish -f net9.0-browserwasm -c Release
   ```

4. Measure size:
   ```powershell
   .\.github\scripts\measure-package-size.ps1 `
     -PublishPath "TestApp\bin\Release\net9.0-browserwasm\publish" `
     -Platform "wasm" `
     -Template "blank" `
     -DotNetVersion "9.0" `
     -UnoVersion "5.5.0" `
     -BuildTime 60 `
     -OutputPath "test-metrics.json"
   ```

## Common Scenarios

### Test a Specific Uno.Templates Version

```yaml
# In workflow dispatch:
uno_template_version: 5.4.10
dotnet_versions: 9.0
templates: blank,recommended
```

### Track Only WebAssembly

Temporarily modify the workflow to only include WASM in the matrix, or create a separate workflow file.

### Investigate Size Increase

When you receive a size alert:

1. Check the GitHub issue created by the workflow
2. Download both current and previous metrics from Azure Storage
3. Compare the detailed JSON files
4. Look for changes in:
   - Assembly count/sizes
   - Largest file changes
   - File count differences

### Export Data for Analysis

```bash
# Download all metrics for a month
az storage blob download-batch \
  --account-name unotemplatemetrics \
  --source template-size-tracking \
  --destination ./metrics-data \
  --pattern "2025/10-*"
```

Then analyze with PowerShell:

```powershell
# Load all metrics
$metrics = Get-ChildItem -Path ./metrics-data -Filter *.json -Recurse | 
    ForEach-Object { Get-Content $_.FullName | ConvertFrom-Json }

# Calculate average size by platform
$metrics | Group-Object platform | 
    Select-Object Name, @{N='AvgSize(MB)';E={($_.Group | Measure-Object packageSize -Average).Average / 1MB}} |
    Format-Table -AutoSize

# Find largest packages
$metrics | 
    Select-Object template, platform, @{N='Size(MB)';E={$_.packageSize / 1MB}} |
    Sort-Object Size -Descending |
    Select-Object -First 10 |
    Format-Table -AutoSize
```

## Troubleshooting

### Workflow fails with Azure credentials error

Check:
```bash
# Verify secrets are set
# Go to Settings → Secrets and variables → Actions

# Test credentials locally
az storage container list --account-name $AZURE_STORAGE_ACCOUNT_NAME --account-key $AZURE_STORAGE_ACCOUNT_KEY
```

### No iOS builds on macos-latest

Check:

- Xcode version on runner
- iOS workload installation
- Code signing certificates (required for device builds)

### Build times out

- Check specific platform build logs
- Consider splitting into separate workflows
- Adjust runner specs if self-hosted

## Maintenance

### Cleanup Old Data

```bash
# Delete metrics older than 90 days
$cutoffDate = (Get-Date).AddDays(-90).ToString("yyyy/MM/dd")

# List blobs to delete
az storage blob list \
  --account-name unotemplatemetrics \
  --container-name template-size-tracking \
  --query "[?properties.lastModified<'$cutoffDate'].name" \
  --output tsv

# Delete (be careful!)
# az storage blob delete-batch ...
```

### Update Uno Templates

The workflow automatically uses the latest prerelease version. To pin to a specific version, set it in workflow dispatch.

## Next Steps

1. **Set up automated cleanup** for old metrics
2. **Create dashboards** using Azure Storage data
3. **Add more platforms** (e.g., Tizen, specific Linux distros)
4. **Integrate with CI/CD** for pull requests
5. **Add performance benchmarks** alongside size tracking

## Support

For issues or questions:
- Open an issue in the repository
- Check the workflow logs in Actions tab
- Review Azure Storage logs for upload issues
