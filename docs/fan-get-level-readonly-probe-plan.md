# FanGetLevel Read-Only Probe Plan

## Why FanGetLevel Is Next

`FanGetLevel` (`CommandType 0x2D`, `hpqBIOSInt128`) is the next read-only fan status candidate after `FanGetCount` and `FanMaxGet`. References show it as the V1/default fan level read path with four zero input bytes and a 128-byte response, separate from the forbidden `SetFanLevel` / `0x2E` write command.

## Required Flags And Elevation

Any future real invocation must require `--hp-victus`, `--hp-wmi-readonly-test`, and an elevated Administrator process.

## Expected Safety Gates

- HP Victus mode enabled
- explicit read-only test mode enabled
- process elevated
- exact `FanGetLevel` command definition
- `SafeReadOnlyInvocation` catalog safety
- `ReadOnly` command access
- `0x2D` command ID
- `hpqBIOSInt128` method and 128-byte output match
- single-shot invocation only
- no raw binary logging

## Expected Report Fields

- `FanGetLevelInvocationAllowed`
- `FanGetLevelInvocationAttempted`
- `FanGetLevelInvocationSucceeded`
- `FanGetLevelReturnedByteCount`
- `FanGetLevelInvocationError`
- `FanGetLevelDecodeSucceeded`
- `FanGetLevelDecodeErrors`
- `FanGetLevelDecoded.Fan1RawValue`
- `FanGetLevelDecoded.Fan2RawValue`
- `FanGetLevelDecoded.RawValueBytes`
- `FanGetLevelDecoded.UnknownByteCount`
- `FanGetLevelDecoded.UnknownByteRange`
- `FanGetLevelDecoded.UnknownNonZeroByteCount`

## Why Raw-Only Decoding Is Required

The `0x2D` values are not proven to be RPM, percent, curve points, or control levels on this Victus. Until real captured bytes are compared with independent fan telemetry, the decoder must preserve only raw per-fan bytes and summarize the unknown tail.

## Still Forbidden

Fan control, fan writes, fan speed control, `SetFanLevel` / `0x2E`, `SetFanMode`, `FanMaxSet` / `0x27`, ambiguous `0x37`, performance mode control, EC access, BIOS writes, hardware writes, polling loops, retries, and ASUS behavior changes remain forbidden.

## Later Manual Test Step

Run this later from an elevated Administrator terminal only after explicit approval:

```powershell
dotnet run --project app\VictusX.csproj -- --hp-victus --hp-wmi-readonly-test
```

Do not run this command from Codex.
