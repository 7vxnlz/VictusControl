# SetFanMax Readiness Panel Audit

This audit verifies the HP Diagnostic dashboard's SetFanMax readiness panel is diagnostic-only. It does not authorize or enable fan writes.

## Data Source

The panel is built from `HpDiagnosticDashboardInput`, populated in `Settings.cs` from the in-memory HP capability snapshot and the cached `hp-capability-report.json`. It reads existing dry-run/report fields only:

- `SetFanMaxWriteImplemented`
- `SetFanMaxWriteAllowed`
- `SetFanMaxDeviceValidatedInputLength`
- `SetFanMaxDryRunBlockedReasons`
- `SetFanMaxNextRequiredProof`
- `SetFanMaxFirstWriteGateStatus`
- `SetFanMaxFirstWriteGateSatisfied`
- `SetFanMaxFirstWriteGateReason`

No panel action refreshes hardware, invokes WMI, runs explicit probes, or creates a write payload.

## Fields Shown

The formatter adds a `SetFanMax evidence readiness` section with:

- current status: `NO-GO`
- fan write implemented: fail-closed false wording
- fan write allowed: fail-closed blocked wording
- `DeviceValidatedInputLength`: unset/not validated unless a cached value is present
- payload length decision: `Not selected`
- missing proof rows for exact payload length, restore behavior, thermal observation, AC/power state, failure/recovery, and human approval
- cached blocked reason and next required proof when available

## Fallback Behavior

Missing or blank values render as `Not available`. Missing, invalid, or unapproved `DeviceValidatedInputLength` renders as `Unset / not validated`. Even cached `1` or `4` is shown only as reported/not approved, and the payload decision remains `Not selected`.

## Copy And Export

Copy summary and export use `HpDiagnosticDashboardFormatter.BuildSummary()` over the same read-only rows shown in the dashboard. They include the SetFanMax readiness rows and do not include raw binary dumps or write instructions.

## Read-Only Safety

The panel is rendered by formatter/model helpers and WinForms labels. It adds no write buttons, sliders, toggles, fan-speed controls, SetFanMax execution path, payload creation, or WMI invocation path. Existing safe actions remain local-only: copy summary, reload cached report, open diagnostic folder, and export diagnostic report.

Source inspection found no `hpqBIOSInt*` invocation added by the readiness panel wiring. The only WMI invocation client remains outside this UI path and is still gated by the explicit developer read-only test flow.

## Current Status

SetFanMax remains **NO-GO**. The panel fails closed for missing or inconsistent gate fields, reports the gate as not satisfied, and does not treat cached data as authorization. Fan writes are not implemented or allowed, `DeviceValidatedInputLength` remains unset for this device, and no payload length has been selected.

## Recommended Next Safe Task

Keep collecting and reviewing documentation-only proof using the SetFanMax proof gap and manual evidence capture documents. The [first-write decision gate](set-fan-max-first-write-decision-gate.md) remains the consolidated NO-GO threshold. Do not implement any fan write code until every gate requirement is independently proven and separately approved.
