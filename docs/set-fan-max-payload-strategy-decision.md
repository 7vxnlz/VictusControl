# SetFanMax Payload Strategy Decision

## Current Evidence

On HP Victus `16-s0035nt`, SKU `7Z5Z2EA#AB8`, BIOS `F.31`, ThermalPolicyVersion `1`:

- Four byte (`01-00-00-00` / `00-00-00-00`) has two exact-device physical-response records. Enable and restore returned success; FanMaxGet stayed `false`.
- One byte (`01` / `00`) has one exact-device physical-response record. Enable and restore returned success; FanMaxGet stayed `false`.
- Raw FanGetLevel changes are raw-only and inconclusive. FanMaxGet is not a reliable sole success criterion.

The references remain split: OmenXHub/OmenSuperHub use one byte, while omencore/ghelper-omen use four bytes. Four byte has the closer Victus/V1 reference context and more exact-device confirmation records.

## Interpretation

The two shapes appear behaviorally similar so far only in observed short-term fan response. That does not prove ABI equivalence, identical firmware interpretation, durable restore behavior, or safe repetition. Neither payload is selected or validated for normal control.

## Developer Experiment Strategy

- Keep both shapes behind separate explicit approval gates and all existing identity, elevation, AC, baseline, single-attempt, matching-restore, and append-only logging gates.
- Prefer four byte for any further controlled confirmation because it has two exact-device records and closer Victus/V1 reference context.
- Permit one byte only as a separately approved comparison path, never as a default or fallback.
- Never retry an attempt or switch payloads in the same run.

## Normal Control Strategy

Normal/user-facing fan control remains **NO-GO**: no UI, sliders, automatic writes, background control, or `DeviceValidatedInputLength` update. Experimental response evidence is not a normal-control contract.

## Experiment Outcome Classification

Experiment records now separately preserve enable/restore command success, FanMaxGet enable confirmation, optional human physical-response and restore observations, and a readback-reliability value. A record with successful commands, recorded physical response, and `FanMaxGet=false` classifies as `CommandSucceededPhysicalResponseObservedReadbackInconclusive`. This is experimental interpretation only, not payload validation. Missing manual observations remain `CommandSucceededNoPhysicalConfirmation`; restore failure and unsafe abort classifications remain failures.

For a separately approved developer experiment only, the operator may record `--physical-fan-response-observed=true|false`, `--restore-observed=true|false`, and bounded `--manual-observation-notes="..."`. These log-only arguments do not pass an approval gate, select a payload, or trigger hardware behavior. Omitted observations remain unknown; they are never inferred from FanMaxGet or raw FanGetLevel values.

The [classified four-byte result](set-fan-max-4byte-classified-experiment-result.md) records command success, physical response, and observed restore with inconclusive FanMaxGet. Its legacy `Outcome` is `Unknown`, not plain `Fail`; the classification remains the authoritative experimental interpretation.

The [clean classified four-byte result](set-fan-max-4byte-clean-classified-result.md) confirms that behavior on a fresh manually observed run. Four byte remains preferred for limited developer experiments only; it is not a normal-control payload selection.

The [four-byte Max Fan Pulse design](set-fan-max-4byte-max-fan-pulse-design.md) now has a separately gated, command-line-only implementation. It preserves the four-byte-only, no-fallback policy and does not alter normal-control NO-GO.

The [pulse result](set-fan-max-4byte-max-fan-pulse-result.md) confirms that this bounded four-byte developer path is operational on the exact device. It does not select a normal-control ABI or change one-byte's comparison-only role.

## Evidence Required Before Normal Control

- Durable, reviewed restore semantics across repeated controlled sessions.
- A reliable success criterion that is not FanMaxGet alone.
- Thermal and AC/battery/power-transition safety evidence.
- Reviewed failure, cancellation, timeout, and recovery behavior.
- A separate product-level decision that selects one payload ABI for normal use.

## Recommended Next Safe Step

Design the evidence and review protocol for repeated controlled sessions and an independent success/restore criterion. Do not add normal controls or change `DeviceValidatedInputLength`.
