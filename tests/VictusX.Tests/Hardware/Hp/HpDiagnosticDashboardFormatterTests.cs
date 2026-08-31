using GHelper.Hardware.Hp;
using Xunit;

namespace VictusX.Tests.Hardware.Hp;

public sealed class HpDiagnosticDashboardFormatterTests
{
    [Fact]
    public void MissingInput_UsesNotAvailableFallbacks()
    {
        IReadOnlyList<HpDiagnosticDashboardSection> sections = HpDiagnosticDashboardFormatter.BuildSections(new());

        HpDiagnosticDashboardSection device = Assert.Single(sections, section => section.Title == "Device");
        Assert.Contains(device.Rows, row => row.Label == "Manufacturer" && row.Value == HpDiagnosticDashboardFormatter.NotAvailable);
        Assert.Contains(device.Rows, row => row.Label == "HP/Victus detected" && row.Value == HpDiagnosticDashboardFormatter.NotAvailable);
    }

    [Fact]
    public void FanSection_KeepsFanGetLevelValuesRawOnly()
    {
        IReadOnlyList<HpDiagnosticDashboardSection> sections = HpDiagnosticDashboardFormatter.BuildSections(new()
        {
            Fan1RawLevel = "23",
            Fan2RawLevel = "0"
        });

        HpDiagnosticDashboardSection fan = Assert.Single(sections, section => section.Title == "Fan read-only status");
        Assert.Contains(fan.Rows, row => row.Label == "Fan 1 raw level byte" && row.Value == "23");
        Assert.Contains(fan.Rows, row => row.Label == "Fan 2 raw level byte" && row.Value == "0");
        Assert.Contains(fan.Rows, row => row.Label == "Raw level data" && row.Value == HpDiagnosticDashboardFormatter.RawFanLevelWarning);
    }

    [Fact]
    public void SafetySection_KeepsNoGoAndNoControlWording()
    {
        IReadOnlyList<HpDiagnosticDashboardSection> sections = HpDiagnosticDashboardFormatter.BuildSections(new()
        {
            SetFanMaxWriteAllowed = "Blocked"
        });

        HpDiagnosticDashboardSection safety = Assert.Single(sections, section => section.Title == "Safety / NO-GO status");
        Assert.Contains(safety.Rows, row => row.Label == "Fan control" && row.Value == HpDiagnosticDashboardFormatter.FanControlStatus);
        Assert.Contains(safety.Rows, row => row.Label == "SetFanMax" && row.Value == HpDiagnosticDashboardFormatter.SetFanMaxStatus);
        Assert.Contains(safety.Rows, row => row.Label == "SetFanMax write allowed" && row.Value == "Blocked" && row.Status == HpDiagnosticDashboardStatus.Blocked);
    }

    [Fact]
    public void Formatter_HasNoWmiDependencyOrInvocationSurface()
    {
        Type[] types = [typeof(HpDiagnosticDashboardFormatter), typeof(HpDiagnosticDashboardInput)];

        Assert.DoesNotContain(
            types.SelectMany(type => type.GetMethods()),
            method => method.Name.Contains("Invoke", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            types.SelectMany(type => type.GetMethods()).SelectMany(method => method.GetParameters()),
            parameter => parameter.ParameterType.Namespace?.Contains("Management", StringComparison.Ordinal) == true);
    }
}
