# SetFanMax Four-Byte Max Fan Pulse Result

## Command

The manually run developer-only pulse was:

```powershell
dotnet run --project app\VictusX.csproj -- --hp-victus --hp-wmi-readonly-test --hp-fan-max-pulse --i-understand-this-can-affect-fans --i-approve-4-byte-max-fan-pulse --physical-fan-response-observed=true --restore-observed=true --manual-observation-notes="Fan ramped for about 2 minutes; airflow increased; restore observed; no abnormal noise, overheating, freeze, or crash."
```

## Recorded Result

- Four-byte enable `01-00-00-00` and matching restore `00-00-00-00` were both attempted and succeeded.
- Post-enable FanMaxGet was `false` with raw FanGetLevel `34-00`.
- Post-restore FanMaxGet was `false` with raw FanGetLevel `34-00`.
- Physical response and restore were observed; no unsafe abort was observed.
- `Outcome=Unknown`, `BlockedReasons=[]`, `WriteExecuted=true`, `FirstWriteGateSatisfied=false`, and `DeviceValidatedInputLength=null`.
- Classification: `CommandSucceededPhysicalResponseObservedReadbackInconclusive`.

## Interpretation

The separately gated four-byte Max Fan Pulse is operational for this exact-device developer experiment: command success, physical fan response, and restore observation were recorded. FanMaxGet remains inconclusive and cannot certify the enabled latch by itself.

This validates only the bounded developer pulse path. It does not fully validate the payload ABI for normal control, set `DeviceValidatedInputLength`, or permit normal/user-facing fan control. One byte remains comparison-only. The Diagnostic UI remains read-only and normal fan control remains **NO-GO**.

## Recommended Next Safe Step

Design either a read-only developer pulse history/status view in the Diagnostic UI or a limited internal service abstraction for future research, without adding user-facing controls.
