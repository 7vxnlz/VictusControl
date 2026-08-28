using VictusControl.Application.Diagnostics;
using VictusControl.Infrastructure.Diagnostics;

namespace VictusControl.Infrastructure.Tests;

public sealed class FileDiagnosticLoggerTests : IDisposable
{
    private readonly string logDirectory;

    public FileDiagnosticLoggerTests()
    {
        logDirectory = Path.Combine(Path.GetTempPath(), "VictusControl.Tests", Guid.NewGuid().ToString("N"));
    }

    [Fact]
    public async Task LogAsync_CreatesLogFileInConfiguredDirectory()
    {
        var logger = new FileDiagnosticLogger(logDirectory);

        await logger.LogAsync(DiagnosticLogLevel.Information, "Test", "Message");

        Assert.True(File.Exists(logger.LogFilePath));
        Assert.StartsWith(Path.GetFullPath(logDirectory), Path.GetFullPath(logger.LogFilePath), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LogAsync_FiltersBelowMinimumLevel()
    {
        var logger = new FileDiagnosticLogger(logDirectory, DiagnosticLogLevel.Warning);

        await logger.LogAsync(DiagnosticLogLevel.Information, "Test", "Filtered");
        await logger.LogAsync(DiagnosticLogLevel.Error, "Test", "Written");

        var content = await File.ReadAllTextAsync(logger.LogFilePath);
        Assert.DoesNotContain("Filtered", content);
        Assert.Contains("Written", content);
    }

    [Fact]
    public void FormatLine_SanitizesMultilineValuesAndSkipsSensitiveProperties()
    {
        var line = FileDiagnosticLogger.FormatLine(
            new DateTimeOffset(2026, 8, 28, 12, 0, 0, TimeSpan.Zero),
            DiagnosticLogLevel.Information,
            "Source\r\nName",
            "Message\r\nText",
            new Dictionary<string, string>
            {
                ["safe"] = "value",
                ["password"] = "secret-value",
                ["token"] = "token-value"
            });

        Assert.DoesNotContain("\r", line);
        Assert.DoesNotContain("\n", line);
        Assert.Contains("Source  Name", line);
        Assert.Contains("Message  Text", line);
        Assert.Contains("safe=value", line);
        Assert.DoesNotContain("secret-value", line);
        Assert.DoesNotContain("token-value", line);
    }

    public void Dispose()
    {
        if (Directory.Exists(logDirectory))
        {
            Directory.Delete(logDirectory, recursive: true);
        }
    }
}
