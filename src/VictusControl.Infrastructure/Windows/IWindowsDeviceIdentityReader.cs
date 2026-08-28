namespace VictusControl.Infrastructure.Windows;

public interface IWindowsDeviceIdentityReader
{
    ValueTask<WindowsDeviceIdentitySnapshot> ReadAsync(CancellationToken cancellationToken = default);
}
