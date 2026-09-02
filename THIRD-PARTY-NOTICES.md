# Third-Party Notices (Draft)

## Draft Status

This is a source-level draft for a future VictusX HP Diagnostic preview. It is not a completed package notice file, does not determine dependency license obligations, and does not authorize publishing. License and notice review for the final runtime package remains pending.

## VictusX Project Notice

VictusX is a modified project based on G-Helper. The repository includes the GNU General Public License version 3 text in `LICENSE`. A future package must include the applicable project license text, the source location and revision used for the package, and this notice only after it has been reviewed against the final package contents.

## G-Helper Attribution

VictusX uses [G-Helper](https://github.com/seerge/g-helper) by seerge as its original application base. The project retains inherited application structure, including the `GHelper` root namespace and resource naming. VictusX is a modified project and is not affiliated with, authorized by, or endorsed by G-Helper.

The reviewed upstream source reference recorded by this repository is commit `5c26f5ac970dab9e26347d80976ebf1eece91b1e`. This attribution does not replace review of applicable upstream license and notice requirements before distribution.

## Runtime Dependency Notice Review (Pending)

The following dependencies are recorded by `app/VictusX.csproj` and the local resolved dependency inventory. They are potential runtime-package candidates. No license or notice conclusion is made here because authoritative package metadata and final artifact contents have not been reviewed.

| Package | Version | Status |
| --- | --- | --- |
| FftSharp | 2.2.0 | Direct runtime dependency; license review pending |
| HidSharpCore | 1.3.0 | Direct runtime dependency; license review pending |
| Microsoft.Management.Infrastructure | 3.0.0 | Direct runtime dependency; license review pending |
| NAudio.Wasapi | 2.3.0 | Direct runtime dependency; license review pending |
| NvAPIWrapper.Net | 0.8.1.101 | Direct runtime dependency; license review pending |
| System.Management | 10.0.10 | Direct runtime dependency; license review pending |
| TaskScheduler | 2.12.2 | Direct runtime dependency; license review pending |
| WinForms.DataVisualization | 1.10.2 | Direct runtime dependency; license review pending |
| Microsoft.Management.Infrastructure.Runtime.Unix | 3.0.0 | Resolved transitive runtime candidate; license review pending |
| Microsoft.Management.Infrastructure.Runtime.Win | 3.0.0 | Resolved transitive runtime candidate; license review pending |
| NAudio.Core | 2.3.0 | Resolved transitive runtime candidate; license review pending |

Before distribution, review each package from authoritative metadata, record the license evidence and required notice text, and compare the reviewed list with the final ZIP or installer contents. See [Dependency Notice Inventory](docs/dependency-notice-inventory.md) and [Package License Review Workflow](docs/package-license-review-workflow.md).

## Test-Only Dependency Notice Review (Separate)

The following packages are recorded only in the test project or its resolved test tooling graph. They are not assumed to be part of an HP Diagnostic preview package. Review them separately only if test tooling, a developer bundle, or another artifact containing them is distributed.

| Package | Version | Status |
| --- | --- | --- |
| Microsoft.NET.Test.Sdk | 18.0.1 | Direct test dependency; license review pending if distributed |
| xunit | 2.9.3 | Direct test dependency; license review pending if distributed |
| xunit.runner.visualstudio | 3.1.5 | Direct test dependency; license review pending if distributed |
| Microsoft.CodeCoverage | 18.0.1 | Resolved test transitive; license review pending if distributed |
| Microsoft.TestPlatform.ObjectModel | 18.0.1 | Resolved test transitive; license review pending if distributed |
| Microsoft.TestPlatform.TestHost | 18.0.1 | Resolved test transitive; license review pending if distributed |
| Newtonsoft.Json | 13.0.3 | Resolved test transitive; license review pending if distributed |
| xunit.abstractions | 2.0.3 | Resolved test transitive; license review pending if distributed |
| xunit.analyzers | 1.18.0 | Resolved test transitive; license review pending if distributed |
| xunit.assert | 2.9.3 | Resolved test transitive; license review pending if distributed |
| xunit.core | 2.9.3 | Resolved test transitive; license review pending if distributed |
| xunit.extensibility.core | 2.9.3 | Resolved test transitive; license review pending if distributed |
| xunit.extensibility.execution | 2.9.3 | Resolved test transitive; license review pending if distributed |

## Icon Attribution (Pending)

The current inherited icon is not approved for the future VictusX preview. A replacement icon's ownership, license, provenance, and any required attribution must be recorded before it is included in a package. See [VictusX Icon Asset Requirements](docs/victusx-icon-asset-requirements.md).

## Trademark Notice

HP, OMEN, and Victus names are used only to identify compatibility targets and research context. VictusX is not affiliated with, authorized by, endorsed by, or certified by HP Inc. No statement in this draft implies an HP or OMEN endorsement.

## Preview Release Blocker

This document is a draft, not completed release evidence. A preview package remains blocked until authoritative runtime dependency license/notice review, final artifact-content comparison, upstream attribution review, icon attribution, signing/checksum evidence, and clean-machine validation are complete. Normal/user-facing fan control also remains NO-GO.
