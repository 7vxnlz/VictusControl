namespace VictusControl.Application.Diagnostics;

public interface IDiagnosticLogger
{
    bool IsEnabled(DiagnosticLogLevel level);

    ValueTask LogAsync(
        DiagnosticLogLevel level,
        string source,
        string message,
        IReadOnlyDictionary<string, string>? properties = null,
        CancellationToken cancellationToken = default);
}
