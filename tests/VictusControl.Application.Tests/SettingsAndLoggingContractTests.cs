using System.Reflection;
using VictusControl.Application.Diagnostics;
using VictusControl.Application.Settings;

namespace VictusControl.Application.Tests;

public sealed class SettingsAndLoggingContractTests
{
    [Fact]
    public void DefaultSettings_AreSafeAndMinimal()
    {
        var settings = AppSettings.Default;

        Assert.Equal(SettingsVersion.Current, settings.Version);
        Assert.False(settings.StartMinimized);
        Assert.False(settings.EnableDiagnosticLogging);
        Assert.Equal(DiagnosticLogLevel.Information, settings.LogLevel);
        Assert.Null(settings.LastSelectedProfileName);
    }

    [Fact]
    public void AppSettings_DoesNotContainHardwareControlOptions()
    {
        var forbiddenTerms = new[] { "Fan", "Thermal", "Wmi", "EmbeddedController", "Bios", "Hardware" };
        var propertyNames = typeof(AppSettings)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name);

        foreach (var propertyName in propertyNames)
        {
            Assert.DoesNotContain(forbiddenTerms, term => propertyName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void SettingsVersion_FormatsDeterministically()
    {
        Assert.Equal("1.0.0", SettingsVersion.Current.ToString());
    }
}
