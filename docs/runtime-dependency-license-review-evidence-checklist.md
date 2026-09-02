# Runtime Dependency License Review Evidence Checklist

## Scope

This checklist defines the evidence required before runtime dependency license and notice review can be marked complete for a future VictusX HP Diagnostic preview. It is source-level planning only. It is not legal advice, does not modify license terms, and does not authorize publishing.

## Current Candidate List Source

The current runtime dependency candidate list comes from:

- `app/VictusX.csproj` direct application `PackageReference` entries;
- locally restored dependency information summarized in [Dependency Notice Inventory](dependency-notice-inventory.md);
- the source-level draft [Third-Party Notices](../THIRD-PARTY-NOTICES.md).

No final preview ZIP or installer exists yet, so package-content matching cannot be completed.

## Direct Runtime Package Candidates

| Package | Version | Current status |
| --- | --- | --- |
| FftSharp | 2.2.0 | Pending review |
| HidSharpCore | 1.3.0 | Pending review |
| Microsoft.Management.Infrastructure | 3.0.0 | Pending review |
| NAudio.Wasapi | 2.3.0 | Pending review |
| NvAPIWrapper.Net | 0.8.1.101 | Pending review |
| System.Management | 10.0.10 | Pending review |
| TaskScheduler | 2.12.2 | Pending review |
| WinForms.DataVisualization | 1.10.2 | Pending review |

## Resolved Transitive Runtime Package Candidates

| Package | Version | Current status |
| --- | --- | --- |
| Microsoft.Management.Infrastructure.Runtime.Unix | 3.0.0 | Pending review |
| Microsoft.Management.Infrastructure.Runtime.Win | 3.0.0 | Pending review |
| NAudio.Core | 2.3.0 | Pending review |

## Required Evidence Per Dependency

For each direct and resolved transitive runtime package candidate, record:

- authoritative package identity: package id, package source, package URL, and upstream repository URL when available;
- exact version reviewed;
- license source: NuGet metadata, package `.nuspec`, bundled license file, upstream repository license, or other authoritative package evidence;
- notice/source distribution requirement visible from the reviewed evidence;
- whether license text must be bundled with the preview package;
- whether attribution or notice text is required;
- whether the package appears in the final artifact;
- package hash or package path from restore metadata when available;
- reviewer name or handle and review date;
- unresolved questions and final decision: reviewed, excluded from artifact, or blocked.

Use `pending review` until this evidence is recorded from authoritative sources.

## Source-Level Draft Versus Release Evidence

`THIRD-PARTY-NOTICES.md` is currently a source-level draft. It lists discovered runtime candidates and marks license conclusions pending. It becomes release evidence only after every runtime candidate is reviewed, required notices are filled in, and the result is compared with the final package contents.

The draft must not be treated as complete merely because a package appears in project files or local restore metadata.

## Final Artifact Inspection Requirement

Before release, inspect the final ZIP or installer contents and confirm:

- every bundled runtime package or dependency file is represented in the reviewed notice record;
- packages listed only in source metadata but absent from the artifact are marked absent with evidence;
- test-only packages are excluded from application notices unless they are actually distributed;
- no developer-only logs, local machine paths, captured device evidence, or experimental command outputs are bundled;
- `LICENSE`, reviewed third-party notices, G-Helper attribution, user safety notes, and icon attribution are included when required.

## Package Manager Metadata Limitations

Project files and restore assets can identify package names, versions, and dependency relationships. They do not by themselves prove license obligations, notice text, source distribution requirements, or final artifact contents.

NuGet metadata may point to a license expression, license file, package readme, repository, or project site, but each source must be reviewed and recorded. If metadata is missing, contradictory, or inaccessible, the dependency remains blocked.

## Fail-Closed Rules

- Unknown license evidence keeps the runtime dependency review blocked.
- Unknown notice or attribution requirements keep the runtime dependency review blocked.
- Missing final artifact inspection keeps the runtime dependency review blocked.
- A dependency found in the final package but absent from the reviewed inventory blocks release.
- A dependency with unresolved metadata conflict blocks release.
- A package removed from the final artifact should be marked excluded only after package inspection confirms it is absent.

## Moving THIRD-PARTY-NOTICES.md From Draft To Reviewed

`THIRD-PARTY-NOTICES.md` may move from draft to reviewed only when:

- every runtime candidate has authoritative license and notice evidence recorded;
- required license text and attribution text are added or explicitly marked not required based on reviewed evidence;
- final artifact contents are inspected and matched to the reviewed inventory;
- inherited G-Helper attribution and applicable project license text are preserved;
- icon attribution is added or explicitly marked not required after the future icon asset is approved;
- reviewer and review date are recorded;
- remaining questions are closed or the release remains blocked.

## Remaining Blocked Items

Runtime dependency review is still incomplete. Final package contents are unavailable, authoritative package licenses and notices are not reviewed, icon attribution depends on a future approved VictusX icon asset, signing/checksum evidence is incomplete, and clean-machine validation has not been run against a package candidate.

Normal/user-facing fan control also remains NO-GO and must not be presented as part of preview readiness.

## Recommended Next Safe Task

Review each direct and transitive runtime dependency against authoritative package metadata and record the evidence in or alongside `THIRD-PARTY-NOTICES.md`; keep the preview release blocked until final artifact inspection also passes.
