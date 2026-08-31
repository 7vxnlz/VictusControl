# SetFanMax Payload And Preflight Plan

This is a pure-data safety design. It does not authorize or execute a hardware write.

## Reference-Backed Shape

`ghelper-omen` documents `SetFanMax` / `0x27` as four bytes: byte 0 is `1` to enable max fan or `0` to restore/disable it; bytes 1-3 are zero. `HpFanMaxPayloadDescription` records those fields only. It never creates a byte buffer, calls WMI, or identifies a callable method.

## Required Preflight

- The requested command must be exactly `SetFanMax` / `0x27`.
- All future explicit write flags must be present and the process must be elevated as Administrator.
- A successful immediate `FanMaxGet` pre-read is required. Its state must be known and disabled for this single enable-then-restore experiment.
- The only experiment target is enable max fan. Its required restore target is restore/disable max fan.
- A post-write `FanMaxGet` readback plan and a verified synchronous restore plan are mandatory.

## Future Flags

- `--hp-victus`
- `--hp-fan-write-experiment`
- `--hp-wmi-write-manual-test`
- `--hp-fan-write-acknowledge-risk`

## Abort Conditions

Abort before any future write for another command, a missing flag, no elevation, no pre-read, unknown or enabled baseline max-fan state, no target, no post-read plan, or no restore plan. After a future attempt, abort without retry or fallback if the call errors, readback does not match, the UI closes, cancellation occurs, or restoration cannot be verified.

## Why No Write Is Executed Yet

The pure evaluator can mark a complete hypothetical request as preflight-approved, but the experiment plan remains `IsWriteExecutionAllowed=false` and has no runtime consumer. There is no WMI call, payload buffer, method selection, allowlist change, or write path in this work.

Reference evidence: `ghelper-omen` `1694844d2725e79a2b2065a0a1494fa1d143e3f4`.
