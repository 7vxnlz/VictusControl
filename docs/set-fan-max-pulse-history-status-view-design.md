# SetFanMax Pulse History Status View Design

## Purpose And Data Source

The HP Diagnostic dashboard may later show a read-only summary of local, append-only SetFanMax records from `%APPDATA%\VictusX\Logs\FanExperiments\`. Its purpose is manual evidence review, not pulse execution or fan control.

## Summary Fields

For the latest valid pulse or experiment record, show:

- Timestamp and payload hypothesis.
- `WriteExecuted`.
- `EnableCommandSucceeded` and `RestoreCommandSucceeded`.
- `PhysicalFanResponseObserved` and `RestoreObserved`.
- `ReadbackReliability` and `ExperimentalOutcomeClassification`.
- A bounded, sanitized notes summary.

The view must label FanMaxGet and raw FanGetLevel as inconclusive/raw when applicable. It must clearly distinguish operational developer-pulse evidence from normal-control validation.

## Fail-Closed Behavior

Missing, unreadable, malformed, schema-unknown, or incomplete logs show "No valid pulse history available" and no optimistic status. Old records must retain unknown values rather than infer a physical response, restore, or approval. Loading is local-file-only and must not invoke WMI or refresh hardware.

## No-Control Boundary

The view must contain no pulse button, fan slider, fan toggle, automatic write, retry, background action, or WMI invocation. It must not expose SetFanMode, SetFanLevel, `0x37`, EC access, or any normal fan-control route.

## Research Value

Local history makes bounded developer results easier to compare across sessions, documents readback limitations, and highlights missing restore or observation evidence. It cannot select a payload, set `DeviceValidatedInputLength`, or grant user-facing control.

## Recommended Next Implementation Step

Add a pure local-log reader and dashboard formatter with fixture-based tests for valid, missing, malformed, and old records. Keep the UI labels-only and leave all command-line pulse execution outside the dashboard.
