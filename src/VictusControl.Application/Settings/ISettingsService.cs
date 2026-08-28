namespace VictusControl.Application.Settings;

public interface ISettingsService
{
    ValueTask<AppSettings> LoadAsync(CancellationToken cancellationToken = default);

    ValueTask SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
