using VictusControl.Domain;
using VictusControl.Hardware.Abstractions;

namespace VictusControl.Application;

public sealed class CapabilityDiscoveryService : ICapabilityDiscoveryService
{
    private readonly IDeviceIdentityProvider identityProvider;
    private readonly IDeviceCapabilityProvider capabilityProvider;

    public CapabilityDiscoveryService(
        IDeviceIdentityProvider identityProvider,
        IDeviceCapabilityProvider capabilityProvider)
    {
        this.identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
        this.capabilityProvider = capabilityProvider ?? throw new ArgumentNullException(nameof(capabilityProvider));
    }

    public async ValueTask<DeviceCapabilityProfile> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var identity = await identityProvider.GetCurrentAsync(cancellationToken).ConfigureAwait(false);
        return await capabilityProvider.GetCapabilitiesAsync(identity, cancellationToken).ConfigureAwait(false);
    }
}
