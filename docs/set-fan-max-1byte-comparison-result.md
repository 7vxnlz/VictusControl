# SetFanMax One-Byte Comparison Result

## Command

The separately approved developer-only comparison command was:

```powershell
dotnet run --project app\VictusX.csproj -- --hp-victus --hp-wmi-readonly-test --hp-fan-write-experiment --set-fan-max-payload-length=1 --i-understand-this-can-affect-fans --i-approve-one-time-set-fan-max-1-byte-comparison
```

Target: Victus by HP Gaming Laptop `16-s0xxx`, SKU `7Z5Z2EA#AB8`, BIOS `F.31`, ThermalPolicyVersion `1`.

## Recorded Result

- Enable `01`: attempted and succeeded.
- Post-enable FanMaxGet: `false`.
- Post-enable raw FanGetLevel: `32-00`.
- Restore `00`: attempted and succeeded.
- Post-restore FanMaxGet: `false`.
- Post-restore raw FanGetLevel: `32-00`.
- Runner outcome: `Fail`, solely because FanMaxGet did not confirm an enabled state.
- `WriteExecuted=true`; `FirstWriteGateSatisfied=false`; `DeviceValidatedInputLength=null`.

The operator observed physical fan operation, increased airflow, and approximately two minutes of apparent max-fan behavior. No abnormal issue was reported.

## Comparison With Four Byte

The [two four-byte experiments](set-fan-max-second-4byte-confirmation-result.md) also returned successful enable/restore calls, produced physical fan ramps for about two minutes, and left FanMaxGet `false`. One byte and four byte therefore appear behaviorally equivalent so far only at the level of observed short-term physical response.

Both raw FanGetLevel results remain raw-only and inconclusive: `32-00` for one byte and `31-00`/`33-00` across the second four-byte record do not prove RPM, percentage, latch state, payload ABI, or restore semantics. FanMaxGet is not reliable as the sole success criterion for this device/BIOS because it remained `false` for every observed response.

## Safety And Decision

One byte now has one exact-device physical-response record; four byte has two. This does not select either payload, validate either input length, or update `DeviceValidatedInputLength`. Normal fan control UI remains **NO-GO** pending a reviewed strategy, reliable success/restore criteria, thermal/power evidence, recovery proof, and product-level UI safety review.

## Recommended Next Safe Step

Define a payload strategy decision that separately governs bounded developer experiments and normal fan control. It must keep normal control disabled until a payload ABI, readback/restore contract, and recovery evidence are independently established.
