using GHelper.Hardware.Hp;
using Xunit;

namespace VictusX.Tests.Hardware.Hp;

public sealed class HpDiagnosticReportExporterTests
{
    [Fact]
    public void Export_WritesTimestampedMarkdownWithReadOnlyDisclaimer()
    {
        string outputDirectory = Path.Combine(Path.GetTempPath(), "VictusX.Tests", Guid.NewGuid().ToString("N"));
        DateTimeOffset timestamp = new(2026, 8, 31, 12, 34, 56, TimeSpan.Zero);

        string filePath = HpDiagnosticReportExporter.Export(
            "Model: Victus\nFan count: 2\nRaw values are not RPM or percent.\nSetFanMax is NO-GO / design-only.",
            timestamp,
            outputDirectory);

        string content = File.ReadAllText(filePath);

        Assert.Equal(Path.Combine(outputDirectory, "hp-diagnostic-20260831-123456.md"), filePath);
        Assert.Contains("cached diagnostic data only", content, StringComparison.Ordinal);
        Assert.Contains("does not invoke WMI", content, StringComparison.Ordinal);
        Assert.Contains("Fan control and fan writes are not implemented", content, StringComparison.Ordinal);
        Assert.Contains("Model: Victus", content, StringComparison.Ordinal);
        Assert.Contains("SetFanMax is NO-GO / design-only.", content, StringComparison.Ordinal);
    }

    [Fact]
    public void Exporter_HasNoWmiDependencyOrInvocationSurface()
    {
        Type exporterType = typeof(HpDiagnosticReportExporter);

        Assert.DoesNotContain(
            exporterType.GetMethods(),
            method => method.Name.Contains("Invoke", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            exporterType.GetMethods().SelectMany(method => method.GetParameters()),
            parameter => parameter.ParameterType.Namespace?.Contains("Management", StringComparison.Ordinal) == true);
    }
}
