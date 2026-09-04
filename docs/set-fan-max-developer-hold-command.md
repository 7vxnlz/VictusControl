# Developer SetFanMax Hold Command

## Status

VictusX includes a developer-only, BIOS-only Max Fan Hold command for bounded exact-device research. It reuses the existing `FourByteMaxFanPulse` contract and SetFanMax runner; it is not normal fan control and has no Settings, tray, or Diagnostic UI route.

Normal/user-facing fan control remains **NO-GO**. `DeviceValidatedInputLength` remains unset, FanMaxGet remains inconclusive, and the one-byte payload remains comparison-only.

## Future Command

```powershell
dotnet run --project app\VictusX.csproj -- --hp-victus --hp-wmi-readonly-test --hp-fan-max-hold --i-understand-this-can-affect-fans --i-approve-4-byte-max-fan-hold --max-fan-hold-seconds=30
```

Optional, bounded manual-observation arguments already supported by the experiment log model may be appended:

```text
--physical-fan-response-observed=true|false
--restore-observed=true|false
--manual-observation-notes="..."
```

Those arguments affect evidence logging and classification only. They cannot satisfy or weaken a gate.

## Gates And Bounds

The parser requires all five explicit flags shown above plus exactly one whole-number duration. The duration is inclusive from `10` through `180` seconds. Missing, duplicate, malformed, zero, negative, shorter, or longer values fail closed before baseline capture or write transport construction.

The reused runtime gates additionally require:

- Administrator elevation and confirmed AC power
- exact HP Victus identity: SKU `7Z5Z2EA#AB8`, BIOS `F.31`, ThermalPolicyVersion `1`
- successful approved read-only baseline
- `FanGetCount=2`
- baseline `FanMaxGet=false`

Any failed gate produces an append-only blocked record with `WriteExecuted=false`.

## Fixed BIOS Operation

The hold accepts no payload-selection argument and uses only:

- WMI class/method: `hpqBIntM` / `hpqBIOSInt0`
- command/type: `0x20008` / `0x27`
- enable: `01-00-00-00`
- restore: `00-00-00-00`

The runner captures the baseline, makes one enable attempt, waits the requested bounded duration, reads FanMaxGet and raw FanGetLevel, and attempts the matching restore plus readback in `finally` whenever enable was attempted. There is no retry, one-byte fallback, EC/PawnIO transport, recurring timer, background hold, or persisted hold state.

The `finally` path covers ordinary completion and exceptions while the process remains alive. It cannot guarantee restore after forced process termination, operating-system failure, power loss, or machine reset; that limitation is one reason this command remains developer-only.

## Evidence And Classification

The append-only JSON record under `%APPDATA%\VictusX\Logs\FanExperiments\` includes the developer-only operation label, requested duration, fixed payload metadata, baseline, enable/restore results, readbacks, optional manual observations, and the existing outcome classification.

If command success, physical response, and restore are recorded while FanMaxGet stays false, the result remains `CommandSucceededPhysicalResponseObservedReadbackInconclusive`. This can describe a bounded developer result; it does not validate a normal-control ABI. `DeviceValidatedInputLength` remains null and `FirstWriteGateSatisfied` remains false.

## Explicitly Excluded

- normal fan UI, sliders, toggles, curves, or pulse/hold buttons
- one-byte selection or fallback
- retries or automatic/background execution
- SetFanMode, SetFanLevel, command type `0x37`, performance writes, or EC writes
- PawnIO/PwnIO, WinRing0, or LibreHardwareMonitor fan-write paths

## Decision

- Developer-only four-byte Max Fan Pulse/Hold: allowed only under separate explicit CLI gates.
- OmenXHub EC/PawnIO fallback: blocked.
- OmenSuperHub one-byte default: not adopted.
- VictusX four-byte research path: preferred, but not validated for normal control.
- Normal/user-facing fan control: **NO-GO**.

The next safe step is source review and pure-test verification of the hold command. Runtime execution requires a separate, explicit operator decision and is outside this implementation task.
