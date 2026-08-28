using VictusControl.Domain;

namespace VictusControl.Application;

public interface ICapabilityDiscoveryService
{
    ValueTask<DeviceCapabilityProfile> DiscoverAsync(CancellationToken cancellationToken = default);
}
