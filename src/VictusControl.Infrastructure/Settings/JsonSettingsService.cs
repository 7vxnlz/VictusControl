using System.Text.Json;
using System.Text.Json.Serialization;
using VictusControl.Application.Settings;

namespace VictusControl.Infrastructure.Settings;

public sealed class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonSettingsService()
        : this(LocalAppDataPaths.SettingsDirectory)
    {
    }

    public JsonSettingsService(string settingsDirectory)
    {
        SettingsDirectory = Path.GetFullPath(settingsDirectory ?? throw new ArgumentNullException(nameof(settingsDirectory)));
        SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");
        EnsureSettingsFilePathIsSafe();
    }

    public string SettingsDirectory { get; }

    public string SettingsFilePath { get; }

    public async ValueTask<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
            {
                return AppSettings.Default;
            }

            await using var stream = File.OpenRead(SettingsFilePath);
            var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken)
                .ConfigureAwait(false);

            return settings ?? AppSettings.Default;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return AppSettings.Default;
        }
    }

    public async ValueTask SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Directory.CreateDirectory(SettingsDirectory);

        await using var stream = File.Create(SettingsFilePath);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    private void EnsureSettingsFilePathIsSafe()
    {
        var fullDirectory = Path.GetFullPath(SettingsDirectory);
        var fullFilePath = Path.GetFullPath(SettingsFilePath);
        var requiredPrefix = fullDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? fullDirectory
            : fullDirectory + Path.DirectorySeparatorChar;

        if (!fullFilePath.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Settings file path must stay inside the settings directory.");
        }
    }
}
