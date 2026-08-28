using VictusControl.Domain;
using VictusControl.Infrastructure.Windows;

namespace VictusControl.Infrastructure.Tests;

public sealed class WindowsDeviceIdentityProviderTests
{
    [Fact]
    public async Task GetCurrentAsync_MapsHpVictusIdentity()
    {
        var provider = new WindowsDeviceIdentityProvider(new FakeWindowsDeviceIdentityReader(
            new WindowsDeviceIdentitySnapshot(
                ManufacturerName: "HP",
                Model: "Victus Gaming Laptop 16-s0035nt",
                SystemFamily: "Victus",
                SystemSku: "7Z5Z2EA",
                ProductName: "Victus by HP Gaming Laptop 16-s0035nt",
                BiosVersion: "F.20")));

        var identity = await provider.GetCurrentAsync();

        Assert.Equal(DeviceManufacturer.Hp, identity.Manufacturer);
        Assert.Equal(DeviceFamily.Victus, identity.Family);
        Assert.Equal("HP", identity.ManufacturerName);
        Assert.Equal("Victus Gaming Laptop 16-s0035nt", identity.Model);
        Assert.Equal("7Z5Z2EA", identity.Sku);
        Assert.Equal("Victus by HP Gaming Laptop 16-s0035nt", identity.ProductName);
        Assert.Equal("F.20", identity.BiosVersion);
        Assert.True(identity.IsKnownHpVictus);
    }

    [Fact]
    public async Task GetCurrentAsync_MapsUnknownManufacturerSafely()
    {
        var provider = new WindowsDeviceIdentityProvider(new FakeWindowsDeviceIdentityReader(
            new WindowsDeviceIdentitySnapshot(
                ManufacturerName: "Contoso",
                Model: "Example Laptop",
                ProductName: "Example Product")));

        var identity = await provider.GetCurrentAsync();

        Assert.Equal(DeviceManufacturer.Other, identity.Manufacturer);
        Assert.Equal(DeviceFamily.Other, identity.Family);
        Assert.Equal("Contoso", identity.ManufacturerName);
        Assert.Equal("Example Laptop", identity.Model);
        Assert.False(identity.IsKnownHpVictus);
    }

    [Fact]
    public async Task GetCurrentAsync_HandlesMissingFieldsWithoutThrowing()
    {
        var provider = new WindowsDeviceIdentityProvider(new FakeWindowsDeviceIdentityReader(
            new WindowsDeviceIdentitySnapshot(
                ManufacturerName: "  ",
                Model: null,
                ProductName: string.Empty)));

        var identity = await provider.GetCurrentAsync();

        Assert.Equal(DeviceIdentity.Unknown, identity);
        Assert.False(identity.IsKnownHpVictus);
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsUnknownWhenReaderFails()
    {
        var provider = new WindowsDeviceIdentityProvider(new ThrowingWindowsDeviceIdentityReader());

        var identity = await provider.GetCurrentAsync();

        Assert.Equal(DeviceIdentity.Unknown, identity);
    }

    [Fact]
    public void Map_DoesNotInferHardwareCapabilities()
    {
        var identity = WindowsDeviceIdentityProvider.Map(new WindowsDeviceIdentitySnapshot(
            ManufacturerName: "HP",
            Model: "Victus Gaming Laptop 16-s0035nt",
            SystemSku: "7Z5Z2EA"));

        var profile = DeviceCapabilityProfile.Unknown(identity);

        Assert.True(identity.IsKnownHpVictus);
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.FanControl));
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.ThermalMode));
        Assert.Equal(CapabilityStatus.Unknown, profile.GetStatus(CapabilityKind.SensorTelemetry));
        Assert.False(profile.IsSupported(CapabilityKind.FanControl));
    }

    private sealed class FakeWindowsDeviceIdentityReader : IWindowsDeviceIdentityReader
    {
        private readonly WindowsDeviceIdentitySnapshot snapshot;

        public FakeWindowsDeviceIdentityReader(WindowsDeviceIdentitySnapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public ValueTask<WindowsDeviceIdentitySnapshot> ReadAsync(CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(snapshot);
    }

    private sealed class ThrowingWindowsDeviceIdentityReader : IWindowsDeviceIdentityReader
    {
        public ValueTask<WindowsDeviceIdentitySnapshot> ReadAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Read failed.");
    }
}
