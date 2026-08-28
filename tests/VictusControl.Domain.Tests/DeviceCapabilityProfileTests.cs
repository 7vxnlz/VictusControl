using VictusControl.Domain;

namespace VictusControl.Domain.Tests;

public sealed class DeviceCapabilityProfileTests
{
    [Fact]
    public void UnknownProfile_DoesNotAssumeCapabilitiesAreSupported()
    {
        var profile = DeviceCapabilityProfile.Unknown();

        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.FanControl));
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.ThermalMode));
        Assert.False(profile.IsSupported(CapabilityKind.FanControl));
        Assert.False(profile.IsSupported(CapabilityKind.ThermalMode));
        Assert.Empty(profile.Capabilities);
    }

    [Fact]
    public void Profile_ReturnsExplicitCapabilityStatusAndConflictSignals()
    {
        var identity = new DeviceIdentity(
            DeviceManufacturer.Hp,
            DeviceFamily.Victus,
            "HP",
            "Victus Gaming Laptop 16-s0035nt");

        var profile = new DeviceCapabilityProfile(
            identity,
            new[]
            {
                new DeviceCapability(CapabilityKind.DeviceIdentity, CapabilityStatus.Supported, "Target identity matched."),
                new DeviceCapability(CapabilityKind.FanControl, CapabilityStatus.Disabled, "Another service may own fan control.")
            },
            new[] { "OMEN Gaming Hub appears to be running." });

        Assert.True(profile.IsSupported(CapabilityKind.DeviceIdentity));
        Assert.Equal(CapabilityStatus.Disabled, profile.GetStatus(CapabilityKind.FanControl));
        Assert.False(profile.IsSupported(CapabilityKind.FanControl));
        Assert.True(profile.HasConflictRisk);
    }
}
