# SetFanMax First-Write Runner Safety Audit

## Scope And Result

Source-level review of the developer-only runner found it is command-line-only and unreachable from the Diagnostic UI, tray menu, and normal startup unless `--hp-fan-write-experiment` is present. The current application wiring deliberately supplies false first-write and human-approval gates, so the runner remains **NO-GO** and blocks before its write transport.

## Verified Gates

- Requires `--hp-victus`, `--hp-wmi-readonly-test`, `--hp-fan-write-experiment`, one `--set-fan-max-payload-length=1` or `=4`, and `--i-understand-this-can-affect-fans`.
- Rejects missing, invalid, duplicate, dry-run, and baseline-capture combinations.
- Requires Administrator elevation and confirmed local AC power; offline or unknown power blocks before baseline capture.
- Requires HP Victus identity, SKU `7Z5Z2EA#AB8`, BIOS `F.31`, thermal policy V1, successful decoded baseline probes, fan count `2`, `FanMaxGet=false`, and nonempty raw FanGetLevel data.
- Captures the approved read-only baseline before the write boundary. It permits one selected hypothesis only, with no retry or alternate-payload fallback.

## Write And Recovery Scope

The runtime transport fixes the only possible command to `0x20008`, type `0x27`, class `hpqBIntM`, method `hpqBIOSInt0`, and the exact paired one-byte or four-byte enable/restore buffers. It has no SetFanMode, SetFanLevel, `0x37`, EC, fan-curve, or performance-control route.

After an enable attempt, the runner waits once, captures FanMaxGet/raw FanGetLevel, then attempts the matching restore in `finally` and captures the same readback again. The audit hardened the exceptional path: a managed transport exception is now treated as an uncertain attempted enable, so matching restore is still attempted once. Append-only `CreateNew` JSON logging preserves baseline, attempts, readbacks, outcome, and blocked/failure reasons.

## Current Boundary

The runner was not executed for this audit, and no WMI method was invoked. The payload length remains unselected, `DeviceValidatedInputLength` remains null, and normal fan control remains absent. Only a separately evidenced change to the documented first-write gate may make an execution review possible.

## Recommended Next Safe Task

Review any future exact-device evidence against the first-write decision gate; retain the runner unexecuted while the gate is **NO-GO**.
