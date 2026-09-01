# SetFanMax Evidence Import and Review Design

## Purpose

This future, documentation-only design accepts a manually completed HP Diagnostic Markdown export as untrusted evidence for review. It does not invoke WMI, execute a write, or change the current **NO-GO** status.

## Intended Input and Fields

The input is an exported diagnostic Markdown file with its manual evidence template completed. A future parser may extract the exact device identity, SystemDesignData summary, timestamped FanGetCount/FanMaxGet/FanGetLevel baselines, AC/battery and thermal observations, payload-length evidence, enable/restore readbacks, recovery notes, reviewer metadata, and human-approval checkpoint.

## Validation and Fail-Closed Behavior

Treat malformed Markdown, missing sections, duplicate or conflicting values, stale or untraceable evidence, non-exact device identity, missing restore proof, and unknown payload length as invalid. The result must report missing or conflicting evidence and remain `NO-GO`; it must never infer values from prose, defaults, repository similarity, or absent fields.

## Limits of Parser Output

Parsing can summarize evidence completeness only. It cannot automatically set the first-write gate to `GO`, grant write permission, set `DeviceValidatedInputLength`, or select one-byte versus four-byte input length. A single imported record is not hardware validation.

## Required Human Review

An independent reviewer must validate the original evidence, exact model/SKU/BIOS match, payload method and length, restore/readback chain, thermal/power conditions, recovery record, and approval wording using the [manual evidence review workflow](set-fan-max-manual-evidence-review-workflow.md). Before implementation, the [first-write decision gate](set-fan-max-first-write-decision-gate.md) and [proof gap checklist](set-fan-max-proof-gap-checklist.md) must be updated together with cited evidence.

## Future Test Strategy

Use pure parser tests for valid structure, missing fields, duplicate/conflicting values, old template versions, malformed Markdown, and fail-closed results. Tests must use fixtures only: no WMI, no hardware, no write payload execution, and no runtime gate mutation.

## Recommended Next Safe Task

Keep the existing export template and review workflow documentation-only. Implement a pure, fixture-based parser only after a separately authorized design task defines a versioned input grammar and reviewer-owned evidence storage.
