using VictusControl.Application;
using VictusControl.Domain;
using VictusControl.Hardware.Abstractions;

namespace VictusControl.Application.Tests;

public sealed class CapabilityDiscoveryServiceTests
{
    [Fact]
    public async Task DiscoverAsync_CombinesIdentityAndCapabilityProviders()
    {
        var identity = new DeviceIdentity(
            DeviceManufacturer.Hp,
            DeviceFamily.Victus,
            "HP",
            "Victus Gaming Laptop 16-s0035nt",
            "7Z5Z2EA");

        var identityProvider = new FakeDeviceIdentityProvider(identity);
        var capabilityProvider = new FakeDeviceCapabilityProvider(identityFromRequest => new DeviceCapabilityProfile(
            identityFromRequest,
            new[]
            {
                new DeviceCapability(CapabilityKind.DeviceIdentity, CapabilityStatus.Supported, "Target identity matched."),
                new DeviceCapability(CapabilityKind.FanControl, CapabilityStatus.Unknown, "Fan control has not been probed.")
            }));

        var service = new CapabilityDiscoveryService(identityProvider, capabilityProvider);

        var profile = await service.DiscoverAsync();

        Assert.Equal(identity, capabilityProvider.RequestedIdentity);
        Assert.True(profile.IsSupported(CapabilityKind.DeviceIdentity));
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.FanControl));
        Assert.False(profile.IsSupported(CapabilityKind.FanControl));
    }

    [Fact]
    public async Task DiscoverAsync_KeepsUnknownHardwareObservationOnly()
    {
        var service = new CapabilityDiscoveryService(
            new FakeDeviceIdentityProvider(DeviceIdentity.Unknown),
            new FakeDeviceCapabilityProvider(DeviceCapabilityProfile.Unknown));

        var profile = await service.DiscoverAsync();

        Assert.Equal(DeviceIdentity.Unknown, profile.Identity);
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.FanControl));
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.ThermalMode));
        Assert.False(profile.IsSupported(CapabilityKind.FanControl));
        Assert.False(profile.IsSupported(CapabilityKind.ThermalMode));
    }

    private sealed class FakeDeviceIdentityProvider : IDeviceIdentityProvider
    {
        private readonly DeviceIdentity identity;

        public FakeDeviceIdentityProvider(DeviceIdentity identity)
        {
            this.identity = identity;
        }

        public ValueTask<DeviceIdentity> GetCurrentAsync(CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(identity);
    }

    private sealed class FakeDeviceCapabilityProvider : IDeviceCapabilityProvider
    {
        private readonly Func<DeviceIdentity, DeviceCapabilityProfile> createProfile;

        public FakeDeviceCapabilityProvider(Func<DeviceIdentity, DeviceCapabilityProfile> createProfile)
        {
            this.createProfile = createProfile;
        }

        public DeviceIdentity? RequestedIdentity { get; private set; }

        public ValueTask<DeviceCapabilityProfile> GetCapabilitiesAsync(
            DeviceIdentity identity,
            CancellationToken cancellationToken = default)
        {
            RequestedIdentity = identity;
            return ValueTask.FromResult(createProfile(identity));
        }
    }
}
