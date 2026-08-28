using VictusControl.Domain;

namespace VictusControl.Hardware.Abstractions;

public interface IDeviceCapabilityProvider
{
    ValueTask<DeviceCapabilityProfile> GetCapabilitiesAsync(
        DeviceIdentity identity,
        CancellationToken cancellationToken = default);
}
