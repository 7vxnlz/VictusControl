using VictusControl.Domain;
using VictusControl.Hardware.Abstractions;
using System.Runtime.Versioning;

namespace VictusControl.Infrastructure.Windows;

public sealed class WindowsDeviceIdentityProvider : IDeviceIdentityProvider
{
    private readonly IWindowsDeviceIdentityReader reader;

    [SupportedOSPlatform("windows")]
    public WindowsDeviceIdentityProvider()
        : this(new WindowsManagementDeviceIdentityReader())
    {
    }

    public WindowsDeviceIdentityProvider(IWindowsDeviceIdentityReader reader)
    {
        this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async ValueTask<DeviceIdentity> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var snapshot = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            return Map(snapshot);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return DeviceIdentity.Unknown;
        }
    }

    public static DeviceIdentity Map(WindowsDeviceIdentitySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var manufacturerName = FirstMeaningful(snapshot.ManufacturerName, snapshot.ProductVendor);
        var model = FirstMeaningful(snapshot.Model, snapshot.ProductName, snapshot.ProductVersion);
        var familyText = FirstMeaningful(snapshot.SystemFamily, snapshot.ProductName, snapshot.Model);
        var sku = FirstMeaningful(snapshot.SystemSku, snapshot.ProductSku);
        var productName = FirstMeaningful(snapshot.ProductName, snapshot.ProductVersion);
        var biosVersion = Normalize(snapshot.BiosVersion);

        var manufacturer = MapManufacturer(manufacturerName);
        var family = MapFamily(manufacturer, familyText, model, productName, sku);

        if (manufacturer == DeviceManufacturer.Unknown &&
            family == DeviceFamily.Unknown &&
            string.IsNullOrEmpty(manufacturerName) &&
            string.IsNullOrEmpty(model) &&
            string.IsNullOrEmpty(sku) &&
            string.IsNullOrEmpty(productName) &&
            string.IsNullOrEmpty(biosVersion))
        {
            return DeviceIdentity.Unknown;
        }

        return new DeviceIdentity(
            manufacturer,
            family,
            manufacturerName,
            model,
            sku,
            productName,
            biosVersion);
    }

    private static DeviceManufacturer MapManufacturer(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DeviceManufacturer.Unknown;
        }

        return Contains(value, "HP") || Contains(value, "Hewlett-Packard")
            ? DeviceManufacturer.Hp
            : DeviceManufacturer.Other;
    }

    private static DeviceFamily MapFamily(
        DeviceManufacturer manufacturer,
        params string[] values)
    {
        if (manufacturer != DeviceManufacturer.Hp)
        {
            return manufacturer == DeviceManufacturer.Unknown
                ? DeviceFamily.Unknown
                : DeviceFamily.Other;
        }

        if (values.Any(value => Contains(value, "Victus")))
        {
            return DeviceFamily.Victus;
        }

        if (values.Any(value => Contains(value, "Omen")))
        {
            return DeviceFamily.Omen;
        }

        return DeviceFamily.Other;
    }

    private static string FirstMeaningful(params string?[] values) =>
        values.Select(Normalize).FirstOrDefault(value => value.Length > 0) ?? string.Empty;

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;

    private static bool Contains(string value, string expected) =>
        value.Contains(expected, StringComparison.OrdinalIgnoreCase);
}
