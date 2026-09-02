# Dependency Notice Inventory

## Scope and Evidence

This source-level inventory uses `app/VictusX.csproj`, `tests/VictusX.Tests/VictusX.Tests.csproj`, and the locally restored `project.assets.json` files under each project's ignored `obj` folder. It is not legal advice. The assets files provide resolved package identities, versions, hashes, and package paths; they do not provide reviewed license or notice conclusions.

No `packages.lock.json`, `.nuspec`, `NOTICE`, or checked-in third-party package notice inventory was found.

## Application Dependencies

These eight direct package references and three resolved transitives are candidates for the future application preview payload. Their license and notice status is unknown from the inspected project and assets metadata.

| Package | Version | Resolution status | Local license/notice evidence |
| --- | --- | --- | --- |
| FftSharp | 2.2.0 | Direct | Not recorded in project/assets metadata |
| HidSharpCore | 1.3.0 | Direct | Not recorded in project/assets metadata |
| Microsoft.Management.Infrastructure | 3.0.0 | Direct | Not recorded in project/assets metadata |
| NAudio.Wasapi | 2.3.0 | Direct | Not recorded in project/assets metadata |
| NvAPIWrapper.Net | 0.8.1.101 | Direct | Not recorded in project/assets metadata |
| System.Management | 10.0.10 | Direct | Not recorded in project/assets metadata |
| TaskScheduler | 2.12.2 | Direct | Not recorded in project/assets metadata |
| WinForms.DataVisualization | 1.10.2 | Direct | Not recorded in project/assets metadata |
| Microsoft.Management.Infrastructure.Runtime.Unix | 3.0.0 | Transitive | Not recorded in project/assets metadata |
| Microsoft.Management.Infrastructure.Runtime.Win | 3.0.0 | Transitive | Not recorded in project/assets metadata |
| NAudio.Core | 2.3.0 | Transitive | Not recorded in project/assets metadata |

## Test-Only Dependencies

The test project's three direct packages resolve ten additional test tooling packages locally. They are not expected to be part of an application preview payload, but need separate review if test tooling or a developer bundle is ever distributed.

| Package | Version | Resolution status |
| --- | --- | --- |
| Microsoft.NET.Test.Sdk | 18.0.1 | Direct |
| xunit | 2.9.3 | Direct |
| xunit.runner.visualstudio | 3.1.5 | Direct |
| Microsoft.CodeCoverage | 18.0.1 | Transitive |
| Microsoft.TestPlatform.ObjectModel | 18.0.1 | Transitive |
| Microsoft.TestPlatform.TestHost | 18.0.1 | Transitive |
| Newtonsoft.Json | 13.0.3 | Transitive |
| xunit.abstractions | 2.0.3 | Transitive |
| xunit.analyzers | 1.18.0 | Transitive |
| xunit.assert | 2.9.3 | Transitive |
| xunit.core | 2.9.3 | Transitive |
| xunit.extensibility.core | 2.9.3 | Transitive |
| xunit.extensibility.execution | 2.9.3 | Transitive |

## Future Preview Package Gate

Follow [Package License Review Workflow](package-license-review-workflow.md) before using this inventory for any preview release decision.

Follow [Package License And Third-Party Notices Completion Plan](package-license-third-party-notices-completion-plan.md) when converting this inventory and the source-level [Third-Party Notices Draft](../THIRD-PARTY-NOTICES.md) into a reviewed package notice file for a future HP Diagnostic preview.

Before a ZIP or installer is created, a maintainer should review authoritative package metadata for every application dependency above, record the applicable license and notice text in a versioned inventory, and confirm the final package contents against that inventory. The package should also include the applicable project license text and the G-Helper modified-project attribution described in [Third-Party Notices Audit](third-party-notices-audit.md).

The test-only list should be reviewed separately if any test or developer tooling is distributed. A future clean restore should regenerate and compare the resolved application graph before the release decision.

## Unknowns and Current Status

This inventory does not establish any dependency's license, redistribution terms, bundled runtime contents, or notice obligations. It also does not replace final package inspection. Release remains blocked by this review work, the inherited icon, signing/checksums, and clean-machine packaged validation.

## Recommended Next Safe Task

Review each runtime entry in the source-level [Third-Party Notices Draft](../THIRD-PARTY-NOTICES.md) with authoritative evidence, then record reviewer, date, required notice text, and final package-content confirmation. Do not publish until the review is complete.
