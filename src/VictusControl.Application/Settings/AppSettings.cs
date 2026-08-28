using VictusControl.Application.Diagnostics;

namespace VictusControl.Application.Settings;

public sealed record AppSettings(
    SettingsVersion Version,
    bool StartMinimized,
    bool EnableDiagnosticLogging,
    DiagnosticLogLevel LogLevel,
    string? LastSelectedProfileName)
{
    public static AppSettings Default { get; } = new(
        SettingsVersion.Current,
        StartMinimized: false,
        EnableDiagnosticLogging: false,
        LogLevel: DiagnosticLogLevel.Information,
        LastSelectedProfileName: null);
}
