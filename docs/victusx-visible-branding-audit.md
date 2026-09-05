# VictusX Visible Branding Audit

## Safe Display Changes

The inherited-shell window title and tray caption now identify the running application as `VictusX`. The existing display-only resource values for “already running” and “open window” now say VictusX in the base English and Turkish resources. Resource keys such as `OpenGHelper` were retained, so existing callers and serialized/configured action mappings are unchanged.

## Intentionally Preserved Compatibility References

- `GHelper` namespaces, `RootNamespace`, `StartupObject`, generated resource namespaces, and resource logical names.
- The `ghelper` custom-action value and related input mapping.
- `Global\\GHelperApp-Exit`, app-data paths, embedded resource logical names, and project/internal type names.
- Updater URLs, updater user-agent text, process/update command assumptions, and inherited documentation/source attribution.
- Existing icon resource names and the final icon wiring plan.

These references are implementation, migration, configuration, upstream-attribution, or updater compatibility concerns rather than safe standalone branding text changes.

## Deferred Visible Text

Localized strings in languages other than English and Turkish still contain inherited product wording. They require language-by-language translation review rather than mechanical replacement. Messages describing inherited ASUS/GPU behavior are also deferred because renaming the product alone could imply that unsupported behavior is available in VictusX.

No namespace, identifier, hardware behavior, dependency, or fan-related code changed.
