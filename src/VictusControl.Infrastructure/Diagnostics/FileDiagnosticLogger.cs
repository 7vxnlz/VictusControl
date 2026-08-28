using System.Text;
using VictusControl.Application.Diagnostics;

namespace VictusControl.Infrastructure.Diagnostics;

public sealed class FileDiagnosticLogger : IDiagnosticLogger
{
    private const int MaxFieldLength = 1024;
    private readonly DiagnosticLogLevel minimumLevel;

    public FileDiagnosticLogger(DiagnosticLogLevel minimumLevel = DiagnosticLogLevel.Information)
        : this(LocalAppDataPaths.LogDirectory, minimumLevel)
    {
    }

    public FileDiagnosticLogger(string logDirectory, DiagnosticLogLevel minimumLevel = DiagnosticLogLevel.Information)
    {
        LogDirectory = Path.GetFullPath(logDirectory ?? throw new ArgumentNullException(nameof(logDirectory)));
        LogFilePath = Path.Combine(LogDirectory, "victuscontrol.log");
        this.minimumLevel = minimumLevel;
    }

    public string LogDirectory { get; }

    public string LogFilePath { get; }

    public bool IsEnabled(DiagnosticLogLevel level) =>
        minimumLevel != DiagnosticLogLevel.None &&
        level != DiagnosticLogLevel.None &&
        level >= minimumLevel;

    public async ValueTask LogAsync(
        DiagnosticLogLevel level,
        string source,
        string message,
        IReadOnlyDictionary<string, string>? properties = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsEnabled(level))
        {
            return;
        }

        Directory.CreateDirectory(LogDirectory);

        var line = FormatLine(DateTimeOffset.UtcNow, level, source, message, properties);
        await File.AppendAllTextAsync(LogFilePath, line + Environment.NewLine, Encoding.UTF8, cancellationToken)
            .ConfigureAwait(false);
    }

    public static string FormatLine(
        DateTimeOffset timestamp,
        DiagnosticLogLevel level,
        string source,
        string message,
        IReadOnlyDictionary<string, string>? properties = null)
    {
        var builder = new StringBuilder();
        builder.Append(timestamp.ToString("O"));
        builder.Append(" [");
        builder.Append(level);
        builder.Append("] ");
        builder.Append(Sanitize(source));
        builder.Append(": ");
        builder.Append(Sanitize(message));

        if (properties is not null)
        {
            foreach (var property in properties
                .Where(property => !IsSensitiveKey(property.Key))
                .OrderBy(property => property.Key, StringComparer.Ordinal))
            {
                builder.Append(" ");
                builder.Append(Sanitize(property.Key));
                builder.Append("=");
                builder.Append(Sanitize(property.Value));
            }
        }

        return builder.ToString();
    }

    private static bool IsSensitiveKey(string key) =>
        key.Contains("password", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("token", StringComparison.OrdinalIgnoreCase);

    private static string Sanitize(string? value)
    {
        var sanitized = (value ?? string.Empty)
            .Replace('\r', ' ')
            .Replace('\n', ' ')
            .Replace('\t', ' ')
            .Trim();

        return sanitized.Length <= MaxFieldLength
            ? sanitized
            : sanitized[..MaxFieldLength];
    }
}
