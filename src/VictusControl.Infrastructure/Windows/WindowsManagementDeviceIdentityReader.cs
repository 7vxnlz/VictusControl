using System.Management;
using System.Runtime.Versioning;

namespace VictusControl.Infrastructure.Windows;

[SupportedOSPlatform("windows")]
public sealed class WindowsManagementDeviceIdentityReader : IWindowsDeviceIdentityReader
{
    public ValueTask<WindowsDeviceIdentitySnapshot> ReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var computerSystem = ReadFirst("SELECT Manufacturer, Model, SystemFamily, SystemSKUNumber FROM Win32_ComputerSystem");
        cancellationToken.ThrowIfCancellationRequested();

        var computerSystemProduct = ReadFirst("SELECT Vendor, Name, Version, SKUNumber FROM Win32_ComputerSystemProduct");
        cancellationToken.ThrowIfCancellationRequested();

        var bios = ReadFirst("SELECT SMBIOSBIOSVersion, Version FROM Win32_BIOS");

        return ValueTask.FromResult(new WindowsDeviceIdentitySnapshot(
            ManufacturerName: Get(computerSystem, "Manufacturer"),
            Model: Get(computerSystem, "Model"),
            SystemFamily: Get(computerSystem, "SystemFamily"),
            SystemSku: Get(computerSystem, "SystemSKUNumber"),
            ProductName: Get(computerSystemProduct, "Name"),
            ProductVendor: Get(computerSystemProduct, "Vendor"),
            ProductVersion: Get(computerSystemProduct, "Version"),
            ProductSku: Get(computerSystemProduct, "SKUNumber"),
            BiosVersion: FirstMeaningful(Get(bios, "SMBIOSBIOSVersion"), Get(bios, "Version"))));
    }

    private static IReadOnlyDictionary<string, string?> ReadFirst(string query)
    {
        using var searcher = new ManagementObjectSearcher("root\\CIMV2", query);
        using var results = searcher.Get();
        using var item = results.Cast<ManagementBaseObject>().FirstOrDefault();

        return item is null
            ? new Dictionary<string, string?>()
            : item.Properties
                .Cast<PropertyData>()
                .ToDictionary(property => property.Name, property => property.Value?.ToString());
    }

    private static string? Get(IReadOnlyDictionary<string, string?> values, string key) =>
        values.TryGetValue(key, out var value) ? value : null;

    private static string? FirstMeaningful(params string?[] values) =>
        values.Select(value => value?.Trim()).FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
}
