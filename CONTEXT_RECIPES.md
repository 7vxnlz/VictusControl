# Context Recipes

Use these recipes to select the smallest useful context for each task. Do not read or pack whole directories without a reason.

## Startup/App Lifecycle

- Start with: app entry point, startup service, dependency registration
- Search for: `Main`, `Startup`, `OnStartup`, `IHostedService`, `Mutex`, `SingleInstance`
- Usually include: startup file, app shell, related tests
- Usually exclude: UI resources, generated files, reference repos
- Use Repomix when: startup touches more than four files
- Recommended model: GPT-5.5 Medium

## Settings/Configuration

- Start with: configuration service, config model, defaults
- Search for: `Configuration`, `Settings`, `AppConfig`, `Load`, `Save`, `Default`
- Usually include: config model, persistence service, tests
- Usually exclude: unrelated UI and hardware implementations
- Use Repomix when: changing schema, defaults, and migration together
- Recommended model: GPT-5.5 Low

## Logging

- Start with: logging abstraction and concrete logger
- Search for: `Logger`, `Logging`, `LogLevel`, `Diagnostics`
- Usually include: logger, diagnostics/export code, tests
- Usually exclude: noisy runtime logs
- Use Repomix when: logging behavior affects multiple services
- Recommended model: Terra Low

## UI/View Model

- Start with: target view model and matching view
- Search for: command names, binding names, property names
- Usually include: view, view model, relevant domain model, UI tests if present
- Usually exclude: hardware details unless directly displayed
- Use Repomix when: a flow crosses several views/view models
- Recommended model: GPT-5.5 Medium

## Hardware Abstraction

- Start with: interfaces and domain models
- Search for: `IHardware`, `IFan`, `IThermal`, `IKeyboard`, `Capabilities`
- Usually include: abstraction, fake/test implementation, consumers
- Usually exclude: vendor-specific implementation unless needed
- Use Repomix when: changing contracts across modules
- Recommended model: GPT-5.5 High

## HP WMI / Hardware Implementation

- Start with: HP hardware implementation and its tests
- Search for: `Wmi`, `Management`, `Cim`, `Bios`, `Victus`, `Capability`
- Usually include: implementation, command encoding tests, safety wrapper
- Usually exclude: UI and unrelated telemetry
- Use Repomix when: comparing command path, safety, and tests together
- Recommended model: GPT-5.6 High

## Telemetry

- Start with: telemetry service, sensor provider, telemetry models
- Search for: `Telemetry`, `Temperature`, `FanRpm`, `Battery`, `PowerSource`, `Poll`
- Usually include: provider, polling coordinator, models, tests
- Usually exclude: view styling and reference repos
- Use Repomix when: telemetry data flows through several layers
- Recommended model: GPT-5.5 Medium

## Safety Behavior

- Start with: safety service, bounds, restore behavior, tests
- Search for: `Safety`, `Clamp`, `Restore`, `Auto`, `Suspend`, `Resume`, `Fallback`
- Usually include: safety logic, hardware abstraction, tests
- Usually exclude: broad UI files
- Use Repomix when: safety spans hardware, application, and lifecycle code
- Recommended model: GPT-5.6 High

## Tests

- Start with: failing test output and target test file
- Search for: tested type name and behavior phrase
- Usually include: test file, implementation file, shared fixtures
- Usually exclude: unrelated test projects
- Use Repomix when: a failure crosses multiple modules
- Recommended model: GPT-5.5 Low

## Build/Package/Release

- Start with: solution, project files, build scripts, GitHub workflows
- Search for: `TargetFramework`, `PackageReference`, `Publish`, `Release`, `Version`
- Usually include: project files, release scripts, CI workflow, packaging docs
- Usually exclude: application internals unless build errors point there
- Use Repomix when: packaging uses multiple scripts/configs
- Recommended model: GPT-5.5 Medium

## Documentation

- Start with: the doc being changed and related workflow files
- Search for: exact heading or term
- Usually include: target docs only
- Usually exclude: source code unless documenting behavior
- Use Repomix when: docs must stay consistent across several files
- Recommended model: Terra Low

## Reference Comparison

- Start with: `REFERENCE_POLICY.md`
- Search for: exact subsystem or behavior in VictusControl first, then reference repos
- Usually include: 1-5 reference files, reviewed commit SHAs, matching VictusControl files
- Usually exclude: whole reference repositories
- Use Repomix when: packaging a small, selected comparison set
- Recommended model: GPT-5.5 High
