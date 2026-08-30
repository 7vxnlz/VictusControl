using System.Management;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace GHelper.Hardware.Hp;

public static class HpVictusCapabilityProbe
{
    private const string CimV2ScopePath = @"\\.\root\cimv2";
    private const string HpWmiScopePath = @"\\.\root\wmi";
    private const string ReportFileName = "hp-capability-report.json";

    private static readonly JsonSerializerOptions ReportJsonOptions = new()
    {
        WriteIndented = true
    };

    public static string ReportPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "VictusX",
        ReportFileName);

    public static HpVictusCapabilitySnapshot Probe()
    {
        var errors = new List<string>();

        var computerSystem = QueryFirst(CimV2ScopePath, "SELECT Manufacturer, Model, SystemFamily, SystemSKUNumber FROM Win32_ComputerSystem", errors, "Win32_ComputerSystem");
        var computerProduct = QueryFirst(CimV2ScopePath, "SELECT Vendor, Name FROM Win32_ComputerSystemProduct", errors, "Win32_ComputerSystemProduct");
        var bios = QueryFirst(CimV2ScopePath, "SELECT SMBIOSBIOSVersion FROM Win32_BIOS", errors, "Win32_BIOS");

        string manufacturer = FirstNonEmpty(computerSystem.GetValueOrDefault("Manufacturer"), computerProduct.GetValueOrDefault("Vendor"));
        string model = FirstNonEmpty(computerSystem.GetValueOrDefault("Model"), computerProduct.GetValueOrDefault("Name"));
        string systemFamily = computerSystem.GetValueOrDefault("SystemFamily") ?? string.Empty;
        string systemSku = computerSystem.GetValueOrDefault("SystemSKUNumber") ?? string.Empty;
        string productVendor = computerProduct.GetValueOrDefault("Vendor") ?? string.Empty;
        string productName = computerProduct.GetValueOrDefault("Name") ?? string.Empty;
        string biosVersion = bios.GetValueOrDefault("SMBIOSBIOSVersion") ?? string.Empty;

        var rootWmiAvailability = CheckNamespace(HpWmiScopePath, errors);
        var hpqBIntMAvailability = rootWmiAvailability == HpVictusProbeAvailability.Available
            ? CheckClass(HpWmiScopePath, "hpqBIntM", errors)
            : HpVictusProbeAvailability.Unknown;
        var hpqBDataInAvailability = rootWmiAvailability == HpVictusProbeAvailability.Available
            ? CheckClass(HpWmiScopePath, "hpqBDataIn", errors)
            : HpVictusProbeAvailability.Unknown;

        return new HpVictusCapabilitySnapshot(
            manufacturer,
            model,
            systemFamily,
            systemSku,
            productVendor,
            productName,
            biosVersion,
            IsHpManufacturer(manufacturer, productVendor),
            IsVictusModel(model, productName, systemSku),
            rootWmiAvailability,
            hpqBIntMAvailability,
            hpqBDataInAvailability,
            errors.ToArray());
    }

    public static string WriteReport(HpVictusCapabilitySnapshot snapshot)
    {
        string reportDirectory = Path.GetDirectoryName(ReportPath) ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        Directory.CreateDirectory(reportDirectory);

        var report = new HpVictusCapabilityReport(
            DateTimeOffset.Now,
            snapshot.Manufacturer,
            snapshot.Model,
            snapshot.SystemFamily,
            snapshot.SystemSku,
            snapshot.BiosVersion,
            snapshot.RootWmiAvailability.ToString(),
            snapshot.HpqBIntMAvailability.ToString(),
            snapshot.HpqBDataInAvailability.ToString(),
            snapshot.IsHpManufacturer,
            snapshot.IsVictusModel,
            snapshot.Errors);

        File.WriteAllText(ReportPath, JsonSerializer.Serialize(report, ReportJsonOptions));
        return ReportPath;
    }

    private static Dictionary<string, string> QueryFirst(string scopePath, string query, List<string> errors, string sourceName)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var searcher = new ManagementObjectSearcher(new ManagementScope(scopePath), new ObjectQuery(query));
            using var results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                using (result)
                {
                    foreach (PropertyData property in result.Properties)
                    {
                        values[property.Name] = property.Value?.ToString()?.Trim() ?? string.Empty;
                    }
                }

                break;
            }
        }
        catch (Exception ex) when (ex is ManagementException or UnauthorizedAccessException or COMException or InvalidOperationException)
        {
            errors.Add($"{sourceName}: {ex.GetType().Name}: {ex.Message}");
        }

        return values;
    }

    private static HpVictusProbeAvailability CheckNamespace(string scopePath, List<string> errors)
    {
        try
        {
            var scope = new ManagementScope(scopePath);
            scope.Connect();
            return scope.IsConnected ? HpVictusProbeAvailability.Available : HpVictusProbeAvailability.Unavailable;
        }
        catch (ManagementException ex) when (ex.ErrorCode == ManagementStatus.InvalidNamespace)
        {
            return HpVictusProbeAvailability.Unavailable;
        }
        catch (Exception ex) when (ex is ManagementException or UnauthorizedAccessException or COMException)
        {
            errors.Add($"{scopePath}: {ex.GetType().Name}: {ex.Message}");
            return HpVictusProbeAvailability.Unknown;
        }
    }

    private static HpVictusProbeAvailability CheckClass(string scopePath, string className, List<string> errors)
    {
        try
        {
            var scope = new ManagementScope(scopePath);
            scope.Connect();

            using var managementClass = new ManagementClass(scope, new ManagementPath(className), null);
            managementClass.Get();
            return HpVictusProbeAvailability.Available;
        }
        catch (ManagementException ex) when (ex.ErrorCode == ManagementStatus.NotFound)
        {
            return HpVictusProbeAvailability.Unavailable;
        }
        catch (Exception ex) when (ex is ManagementException or UnauthorizedAccessException or COMException)
        {
            errors.Add($"{className}: {ex.GetType().Name}: {ex.Message}");
            return HpVictusProbeAvailability.Unknown;
        }
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (string? value in values)
        {
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        }

        return string.Empty;
    }

    private static bool IsHpManufacturer(string manufacturer, string productVendor) =>
        ContainsAny(manufacturer, "HP", "Hewlett-Packard") || ContainsAny(productVendor, "HP", "Hewlett-Packard");

    private static bool IsVictusModel(params string[] values) =>
        values.Any(value => ContainsAny(value, "Victus"));

    private static bool ContainsAny(string value, params string[] candidates) =>
        !string.IsNullOrWhiteSpace(value) && candidates.Any(candidate => value.Contains(candidate, StringComparison.OrdinalIgnoreCase));

    private sealed record HpVictusCapabilityReport(
        DateTimeOffset Timestamp,
        string Manufacturer,
        string Model,
        string SystemFamily,
        string Sku,
        string BiosVersion,
        string RootWmiAvailability,
        string HpqBIntMAvailability,
        string HpqBDataInAvailability,
        bool LooksLikeHp,
        bool LooksLikeVictus,
        string[] ProbeErrors);
}
