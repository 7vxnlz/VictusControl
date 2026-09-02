# VictusX Icon And App Identity Implementation Plan

## Current App Identity State

- `app/VictusX.csproj` builds `VictusX.exe` with `AssemblyName=VictusX`.
- Project metadata already uses `Product=VictusX`, a read-only diagnostic description, `VictusX Contributors`, and `0.1.0-preview.1` / `0.1.0.0` version fields.
- `app/app.manifest` uses `VictusX.app` and `asInvoker`.
- `RootNamespace=GHelper` and `StartupObject=GHelper.Program` remain intentionally inherited for resource and startup compatibility.
- `--hp-victus` mode uses the visible title `VictusX Read-only Diagnostic` and tray text `VictusX`.

There is no `app/Properties/AssemblyInfo.cs`; executable metadata is currently project-file driven.

## Current Icon And Resource State

- `ApplicationIcon` points to `app/favicon.ico`, identified by packaging docs as the inherited G-Helper icon.
- The initial tray icon is `Properties.Resources.standard`.
- `Settings.VisualiseIcon` can later switch the tray icon among inherited `standard`, `eco`, `ultimate`, `light_standard`, `dark_standard`, `light_eco`, and `dark_eco` resources.
- `app/Properties/Resources.resx` contains inherited icon resources from `app/Resources/*.ico`.
- `app/UI/IconHelper.cs` sets the large form icon from the executable associated icon and the small form icon from supplied bitmap/icon resources.

No icon, image, resource, or binary asset is changed by this plan.

## Required Icon Assets And Sizes

Use the acceptance rules in [VictusX Icon Asset Requirements](victusx-icon-asset-requirements.md). The future asset set should include:

- an original or explicitly licensed source master
- a multi-resolution Windows `.ico`
- sizes: 16x16, 20x20, 24x24, 32x32, 40x40, 48x48, and 256x256 where the design supports them cleanly
- documented provenance, author/source, license, and attribution requirements
- light/dark taskbar and high-DPI readability evidence

## WPF/WinForms Resource And App Icon Integration Points

VictusX currently uses Windows Forms, not a WPF application shell. The relevant integration points are:

- `app/VictusX.csproj` `ApplicationIcon`
- `app/favicon.ico`
- optional HP-specific icon resource in `app/Properties/Resources.resx`
- generated `app/Properties/Resources.Designer.cs`
- `app/Program.cs` initial `NotifyIcon.Icon`
- `app/Settings.cs` tray icon refresh logic in `VisualiseIcon`
- `app/UI/IconHelper.cs` executable/window icon behavior
- future publish profile and launcher/shortcut metadata

If an HP-mode-only tray icon is added, keep the inherited default tray/icon behavior unchanged outside `--hp-victus`.

## Tray Icon Integration Considerations

- Add a distinct HP Diagnostic/VictusX resource only after the asset passes the icon requirements checklist.
- Select the VictusX tray icon only when `--hp-victus` is active.
- Guard later inherited tray refresh paths so HP Diagnostic mode does not revert to `Properties.Resources.standard`.
- Preserve Diagnostic/Quit-only tray behavior in HP mode.
- Do not add pulse, fan-control, updater, or ASUS control entries while changing icon behavior.

## Windows Executable Metadata Considerations

- Keep `Product`, `Description`, `Company`, `Authors`, `Copyright`, `Version`, `AssemblyVersion`, `FileVersion`, and `InformationalVersion` project metadata aligned.
- Keep `Description` explicitly read-only for the HP Diagnostic preview.
- Do not change `RootNamespace` or broad namespaces as part of icon work.
- Verify executable properties only from a package candidate after the asset is integrated.

## Branding Constraints

- App name remains `VictusX`.
- HP Diagnostic preview wording remains read-only.
- Do not imply HP, OMEN, Victus, ASUS, ROG, Armoury Crate, or G-Helper endorsement.
- Do not copy or derive from HP, OMEN, Victus, ASUS, ROG, Windows, or inherited G-Helper marks unless explicit permission is documented.
- Preserve upstream and third-party attribution.
- Do not use icon language that suggests normal fan control, fan curves, performance tuning, RGB control, EC writes, BIOS writes, or hardware writes.

## Implementation Steps

1. Approve the icon source, license/provenance record, attribution requirements, and size checklist.
2. Add the reviewed `.ico` asset and source master in a narrowly named location.
3. Update `ApplicationIcon` only to the reviewed VictusX icon when the executable icon replacement is approved.
4. Add an HP Diagnostic-specific resource entry if tray selection needs a resource-backed icon.
5. Add a small `--hp-victus` conditional for tray icon selection and HP-mode tray refresh preservation.
6. Keep default ASUS/G-Helper icon resources and non-HP runtime behavior unchanged.
7. Update release blocker docs and notice docs if the icon introduces attribution requirements.
8. Run build/tests and later packaged visual verification before any preview release.

## Verification Steps

- `dotnet build VictusX.sln`
- `dotnet test VictusX.sln`
- launch normal HP Diagnostic mode with `--hp-victus` only
- confirm title remains `VictusX Read-only Diagnostic`
- confirm Diagnostic/Quit-only HP shell remains intact
- confirm no fan-control UI, pulse button, sliders, toggles, or updater/control surfaces appear
- confirm executable icon in Properties, Explorer, taskbar, Alt-Tab, shortcut, tray, and window surfaces
- confirm default ASUS/G-Helper mode icon behavior is unchanged outside `--hp-victus`
- confirm no release shortcut/profile includes `--hp-wmi-readonly-test`
- perform clean-machine package validation before publishing

## Risks And Rollback Plan

Risks:

- generated resource designer churn
- HP Diagnostic tray refresh accidentally reverting to inherited icons
- default ASUS/G-Helper behavior changing outside `--hp-victus`
- executable and tray icons diverging in packaged builds
- accidental vendor/upstream branding reuse

Rollback:

- revert only the icon asset/resource/project/icon-selection changes from the icon integration task
- leave fan safety, diagnostic data, and packaging docs intact
- re-run build/tests and HP Diagnostic launch after rollback
- keep release blocked until visual identity verification passes

## Recommended Next Safe Task

Create or approve an original VictusX diagnostic icon asset with documented license/provenance and size coverage. Do not integrate it until the asset satisfies the acceptance checklist.
