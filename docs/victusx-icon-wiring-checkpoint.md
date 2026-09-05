# VictusX Icon Wiring Checkpoint

## Current Wiring

- `app/VictusX.csproj` embeds `app/favicon.ico` through `<ApplicationIcon>favicon.ico</ApplicationIcon>` for `VictusX.exe`.
- HP-mode `NotifyIcon` starts with inherited `Properties.Resources.standard` in `app/Program.cs`.
- `Settings.VisualiseIcon` can later replace that tray icon with inherited GPU-mode resources: `standard`, `eco`, `ultimate`, `light_standard`, `dark_standard`, `light_eco`, and `dark_eco`.
- `app/UI/IconHelper.cs` obtains the large window icon from the executable associated icon. Therefore the executable icon will also cover the large form/window surface after `ApplicationIcon` is updated.

## Final Asset Contract

The approved final multi-resolution asset should be added at exactly:

```text
app/Assets/VictusX.ico
```

After approval, wire the same physical file in two places:

1. Change `app/VictusX.csproj` to `<ApplicationIcon>Assets\\VictusX.ico</ApplicationIcon>` and include the file as project content if the project system does not already resolve it.
2. Add `app/Assets/VictusX.ico` to `app/Properties/Resources.resx` as an icon resource, regenerate `Resources.Designer.cs`, and select that resource only for `--hp-victus` in `Program.cs`. Guard `Settings.VisualiseIcon` so later inherited GPU-mode refreshes cannot replace the HP-mode icon.

This gives WinForms tray, executable, taskbar, Explorer, Alt-Tab, and large window identity one approved source asset, while preserving inherited default-mode icon behavior outside HP mode.

## Metadata And Remaining Inheritance

`AssemblyName`, `Product`, description, company, authors, and version metadata already identify VictusX. The default launch profile has been renamed from `GHelper` to `VictusX`; no runtime behavior changes. `RootNamespace=GHelper`, `StartupObject=GHelper.Program`, resource logical names, `favicon.ico`, and the inherited tray/GPU icon resources remain for compatibility until the reviewed asset integration task.

No placeholder icon, resource entry, project icon path, tray selection change, artwork, or package artifact was added. The final asset must satisfy the provenance, licensing, size, and dark/light contrast requirements in [VictusX Icon Asset Requirements](victusx-icon-asset-requirements.md) before integration.
