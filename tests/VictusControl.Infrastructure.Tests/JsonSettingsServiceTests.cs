using VictusControl.Application.Diagnostics;
using VictusControl.Application.Settings;
using VictusControl.Infrastructure.Settings;

namespace VictusControl.Infrastructure.Tests;

public sealed class JsonSettingsServiceTests : IDisposable
{
    private readonly string settingsDirectory;

    public JsonSettingsServiceTests()
    {
        settingsDirectory = Path.Combine(Path.GetTempPath(), "VictusControl.Tests", Guid.NewGuid().ToString("N"));
    }

    [Fact]
    public async Task LoadAsync_ReturnsDefaultsWhenSettingsFileDoesNotExist()
    {
        var service = new JsonSettingsService(settingsDirectory);

        var settings = await service.LoadAsync();

        Assert.Equal(AppSettings.Default, settings);
        Assert.False(File.Exists(service.SettingsFilePath));
    }

    [Fact]
    public async Task LoadAsync_ReturnsDefaultsForCorruptSettings()
    {
        var service = new JsonSettingsService(settingsDirectory);
        Directory.CreateDirectory(settingsDirectory);
        await File.WriteAllTextAsync(service.SettingsFilePath, "{not valid json");

        var settings = await service.LoadAsync();

        Assert.Equal(AppSettings.Default, settings);
    }

    [Fact]
    public async Task SaveAsync_AndLoadAsync_RoundTripsSettings()
    {
        var service = new JsonSettingsService(settingsDirectory);
        var expected = new AppSettings(
            SettingsVersion.Current,
            StartMinimized: true,
            EnableDiagnosticLogging: true,
            LogLevel: DiagnosticLogLevel.Warning,
            LastSelectedProfileName: "Quiet");

        await service.SaveAsync(expected);
        var actual = await service.LoadAsync();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Constructor_KeepsSettingsFileInsideSettingsDirectory()
    {
        var service = new JsonSettingsService(settingsDirectory);
        var fullDirectory = Path.GetFullPath(settingsDirectory);
        var fullFilePath = Path.GetFullPath(service.SettingsFilePath);

        Assert.StartsWith(fullDirectory, fullFilePath, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith("settings.json", fullFilePath, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        if (Directory.Exists(settingsDirectory))
        {
            Directory.Delete(settingsDirectory, recursive: true);
        }
    }
}
