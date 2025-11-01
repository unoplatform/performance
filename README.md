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

### Secret Configuration

Add the following GitHub repository secrets to enable all workflow features:

| Secret Name | Purpose |
|-------------|---------|
| `SIZE_CHECK_AZURE_STORAGE_ACCOUNT_NAME` | Azure Storage Account name for metrics archival |
| `SIZE_CHECK_AZURE_STORAGE_ACCOUNT_KEY`  | Key for the above storage account |
| `UNO_APPLE_PROD_CERT_BASE64`            | Base64-encoded iOS distribution P12 certificate |
| `UNO_APPLE_PROD_CERT_PASSWORD`         | Password for the P12 certificate |
| `SIZE_CHECK_IOS_PROVISION_PROFILE_BASE64` | Base64-encoded `.mobileprovision` profile used for signing |

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

Scripts used:

- `measure-package-size.ps1` — collects sizes, counts, build time.
- `upload-to-azure.ps1` — uploads metrics to Azure blob storage using `AZURE_STORAGE_ACCOUNT` and `AZURE_STORAGE_KEY` environment variables (mapped from the new `SIZE_CHECK_*` secrets).
- `compare-and-alert.ps1` — compares current vs previous day and emits alert / critical flags.
- `generate-summary.ps1` — produces markdown summary output.

Environment thresholds:

- `ALERT_THRESHOLD` (default 10%) — triggers an informational issue.
- `FAILURE_THRESHOLD` (fixed 20%) — fails the workflow for critical regressions.

### Extending

To add more .NET versions or templates, use the `workflow_dispatch` inputs:

```text
dotnet_versions: 9.0,10.0
templates: blank,recommended
```

To add new platforms once Uno templates support them, extend the matrix generation in the `Prepare Build Matrix` step.

### Troubleshooting

| Symptom | Cause | Resolution |
|---------|-------|-----------|
| iOS signing fails with provisioning UUID empty | Invalid or expired provisioning profile | Recreate profile, re-base64 encode, update secret |
| Azure upload fails (Auth error) | Missing or incorrect storage account secrets | Verify `SIZE_CHECK_AZURE_STORAGE_ACCOUNT_NAME` and `SIZE_CHECK_AZURE_STORAGE_ACCOUNT_KEY` |
| No previous metrics downloaded | First run or prior day failed | Ignore if first run; ensure daily schedule succeeded |
| Critical size increase failure | Large artifact growth beyond `FAILURE_THRESHOLD` | Inspect build artifacts; investigate dependency or template changes |

### Security Notes

- Keep certificate and provisioning profile secrets limited to required maintainers.
- Rotate Azure Storage account keys periodically and update corresponding GitHub secrets.
- Avoid echoing secret contents in workflow logs; current scripts only decode to files.

### Future Improvements

- Automate trend chart generation and publish to repository pages.
- Add Android `ApplicationId` alignment if needed for distribution parity.
