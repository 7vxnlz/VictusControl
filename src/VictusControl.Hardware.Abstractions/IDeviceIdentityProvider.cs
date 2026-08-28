using VictusControl.Domain;

namespace VictusControl.Hardware.Abstractions;

public interface IDeviceIdentityProvider
{
    ValueTask<DeviceIdentity> GetCurrentAsync(CancellationToken cancellationToken = default);
}
