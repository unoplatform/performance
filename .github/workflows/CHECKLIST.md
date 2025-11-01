# Deployment Checklist

Use this checklist to deploy the Daily Template Size Tracking workflow.

## Pre-Deployment

- [ ] Review all workflow files in `.github/workflows/`
- [ ] Review all scripts in `.github/scripts/`
- [ ] Read `README.md` and `SETUP.md`
- [ ] Understand the alert thresholds and metrics collected

## Azure Setup

- [ ] Create Azure Storage Account
  ```bash
  az storage account create \
    --name <your-account-name> \
    --resource-group <your-rg> \
    --location <region> \
    --sku Standard_LRS
  ```

- [ ] Get Azure Storage Account Key
  ```bash
  az storage account keys list \
    --account-name <your-account-name> \
    --resource-group <your-rg> \
    --query "[0].value" -o tsv
  ```

- [ ] Note down:
  - Storage Account Name: `________________`
  - Storage Account Key: `________________`

## GitHub Configuration

- [ ] Go to repository Settings → Secrets and variables → Actions
- [ ] Add `AZURE_STORAGE_ACCOUNT_NAME` secret
- [ ] Add `AZURE_STORAGE_ACCOUNT_KEY` secret
- [ ] Verify secrets are saved correctly

## Permissions Check

- [ ] Ensure GitHub Actions has permission to create issues
  - Settings → Actions → General → Workflow permissions
  - Select "Read and write permissions"
  - Check "Allow GitHub Actions to create and approve pull requests"

## First Test Run

- [ ] Go to Actions → Daily Template Size Tracking
- [ ] Click "Run workflow"
- [ ] Use these test parameters:
  - uno_template_version: (leave empty)
  - dotnet_versions: `9.0`
  - templates: `blank`
  - alert_threshold: `10`
- [ ] Click "Run workflow"
- [ ] Monitor the workflow execution

## Verify First Run

- [ ] Check that all jobs completed successfully
- [ ] Review the workflow summary
- [ ] Verify metrics uploaded to Azure Storage:

  ```bash
  az storage blob list \
    --account-name <your-account-name> \
    --container-name template-size-tracking \
    --output table
  ```

- [ ] Download and inspect a metric file:

  ```bash
  az storage blob download \
    --account-name <your-account-name> \
    --container-name template-size-tracking \
    --name "latest/net9.0/blank-android.json" \
    --file test-metric.json
  ```

- [ ] Review the JSON structure in `test-metric.json`

## Full Test Run

- [ ] Run workflow again with full matrix:
  - uno_template_version: (leave empty)
  - dotnet_versions: `9.0,10.0`
  - templates: `blank,recommended`
  - alert_threshold: `10`
- [ ] Verify all 24 jobs complete successfully
- [ ] Check summary shows all platforms
- [ ] Verify Azure Storage has metrics for all combinations

## Comparison Test

- [ ] Wait 24 hours or manually upload "previous" metrics
- [ ] Run workflow again with same parameters
- [ ] Verify comparison works:
  - [ ] Summary shows changes
  - [ ] No false alerts (if sizes unchanged)
  - [ ] Comparison JSON is generated

## Alert Test (Optional)

To test the alerting system:

- [ ] Manually modify a "previous" metric file in Azure to have a smaller size
- [ ] Run the workflow
- [ ] Verify:
  - [ ] Alert is detected in logs
  - [ ] GitHub issue is created
  - [ ] Issue contains proper formatting
  - [ ] Workflow summary shows alert indicator

## Scheduled Run

- [ ] Verify cron schedule is correct: `0 5 * * *` (midnight ET / 5 AM UTC)
- [ ] Wait for first scheduled run
- [ ] Check that it runs automatically at the scheduled time
- [ ] Verify scheduled runs appear in Actions history

## Documentation

- [ ] Create wiki page or README section linking to:
  - Workflow documentation
  - Setup guide
  - Azure Storage location
- [ ] Document any custom configurations made
- [ ] Share access information with team

## Monitoring Setup

- [ ] Set up Azure Storage monitoring (optional)
- [ ] Configure Azure Storage lifecycle policy for old data cleanup (optional)
- [ ] Subscribe to workflow failure notifications:
  - Go to repository → Watch → Custom → Actions

## Post-Deployment

- [ ] Monitor first week of daily runs
- [ ] Review any issues or failures
- [ ] Adjust thresholds if needed
- [ ] Document any learnings or adjustments

## Maintenance Schedule

Set reminders for:

- [ ] Monthly: Review alert frequency and accuracy
- [ ] Quarterly: Clean up old Azure Storage data (>90 days)
- [ ] Annually: Review and update .NET versions in workflow
- [ ] As needed: Update Uno.Templates version pinning if desired

## Troubleshooting Reference

If issues occur, check:

1. **Workflow fails to start**
   - Verify cron syntax
   - Check GitHub Actions permissions

2. **Build failures**
   - Review specific job logs
   - Check .NET SDK availability
   - Verify workload installations

3. **Azure upload fails**
   - Verify secrets are correct
   - Check Azure Storage account access
   - Review Azure CLI version in workflow

4. **No comparisons shown**
   - Verify previous day's data exists in Azure
   - Check date path format
   - Review download previous metrics step

5. **False alerts**
   - Adjust alert threshold
   - Review comparison logic
   - Check for data inconsistencies

## Support Contacts

Document who to contact for:

- Azure Storage issues: `________________`
- GitHub Actions issues: `________________`
- Workflow modifications: `________________`
- Alert triage: `________________`

---

**Status**: ⬜ Not Started | 🟡 In Progress | ✅ Complete

**Date Started**: `________________`

**Date Completed**: `________________`

**Deployed By**: `________________`

**Notes**:
```
[Add any deployment notes, issues encountered, or configuration decisions here]
```
