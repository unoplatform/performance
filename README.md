# Uno Platform Performance

This repository contains standalone benchmarks for the [Uno Platform](https://github.com/unoplatform/uno), based on [Benchmark.NET](https://benchmarkdotnet.org/).

## Related

- [Uno Platform .NET 5 support annoucement, with benchmarks](https://platform.uno/?p=3874).

## Daily Template Size Tracking Workflow

The repository includes an automated GitHub Actions workflow `daily-template-size-tracking.yml` that:

- Runs daily (5 AM UTC) and on pull requests touching the workflow or related scripts.
- Generates Uno template projects (`dotnet new unoapp`) for a matrix of:
  - .NET versions: 9.0 and 10.0 (configurable)
  - Template presets: `blank`, `recommended` (configurable)
  - Platforms: Android, iOS, WebAssembly, Desktop (Windows, Linux, macOS)
- Builds each project in Release and publishes artifacts.
- Collects size & build metrics via PowerShell scripts under `.github/scripts/template-size-tracking/`.
- Uploads metrics JSON files to Azure Blob Storage in a date & framework structured layout.
- Compares current results vs previous day and issues alerts if size deltas exceed thresholds.

### Secrets & Configuration

The workflow uses federated identity (OIDC) for Azure Blob uploads. Configure these GitHub repository secrets:

| Name | Purpose |
|------|---------|
| `SIZE_CHECK_AZURE_STORAGE_ACCOUNT_NAME` | Storage account name for metrics archival |
| `SIZE_CHECK_AZURE_CLIENT_ID` | Client ID of Entra application or user-assigned managed identity |
| `SIZE_CHECK_AZURE_TENANT_ID` | Tenant (directory) ID for the identity |
| `SIZE_CHECK_AZURE_SUBSCRIPTION_ID` | Subscription ID hosting the storage account |
| `UNO_APPLE_PROD_CERT_BASE64` | Base64-encoded iOS distribution P12 certificate |
| `UNO_APPLE_PROD_CERT_PASSWORD` | Password for the P12 certificate |
| `SIZE_CHECK_IOS_PROVISION_PROFILE_BASE64` | Base64-encoded provisioning profile (.mobileprovision) |

### iOS Signing

iOS builds use a local composite action located at `.github/actions/manual-ios-signing/action.yml` which:

1. Decodes the distribution certificate and provisioning profile from secrets.
2. Creates an ephemeral keychain and imports the certificate.
3. Extracts the provisioning profile UUID and exports `PROVISIONING_UUID` and `CODESIGN_KEY` environment variables.
4. The workflow then publishes the iOS project with:
   - `-p:CodesignKey="$env:CODESIGN_KEY"`
   - `-p:CodesignProvision="$env:PROVISIONING_UUID"`
   - `-p:ApplicationId=uno.platform.performance`

For production distribution, the `codesign-key` input is set to `iPhone Distribution`.

### Metrics & Alerts

Scripts:

| Script | Purpose |
|--------|---------|
| `measure-package-size.ps1` | Collects sizes, counts, build time |
| `upload-to-azure.ps1` | Uploads metrics using identity (`--auth-mode login`) |
| `compare-and-alert.ps1` | Compares current vs previous day; sets alert/critical flags |
| `generate-summary.ps1` | Produces markdown summary output |

Thresholds:

| Variable | Default | Meaning |
|----------|---------|---------|
| `ALERT_THRESHOLD` | 10% | Opens issue if exceeded |
| `FAILURE_THRESHOLD` | 20% | Fails workflow if exceeded |

### Extending

Dispatch inputs (example):

```text
dotnet_versions: 9.0,10.0
templates: blank,recommended
```

Add platforms by editing matrix generation in the workflow.

### Troubleshooting

| Symptom | Cause | Resolution |
|---------|-------|-----------|
| iOS signing fails (UUID empty) | Invalid / expired provisioning profile | Recreate, re-base64, update secret |
| Azure upload fails (Auth) | Identity lacks role or wrong account name | Assign `Storage Blob Data Contributor`; verify storage name |
| No previous metrics | First run or prior day failed | Ignore first run; check daily schedule logs |
| Critical size increase failure | Large artifact growth | Inspect build artifacts; dependency changes |

### Security Notes

- Federated identity removes need for long-lived storage keys; use least-privilege RBAC.
- Limit access to certificate & provisioning secrets.
- Secrets are never echoed; scripts decode only locally.
- If a temporary rollback to key auth occurs, remove the key ASAP after returning to identity.

### Future Improvements

- Automate trend chart generation and publish to repository pages.
- Add Android `ApplicationId` alignment if needed.
- Optional SAS generation for secure external sharing of metrics.
