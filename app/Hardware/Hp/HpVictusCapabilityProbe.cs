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

        var hpWmiSnapshot = new HpWmiReadOnlyClient().Probe();
        foreach (string error in hpWmiSnapshot.Errors)
        {
            errors.Add("HP WMI read-only client: " + error);
        }

        var accessDeniedDiagnostics = HpWmiAccessDeniedDiagnostics.Probe();
        foreach (string error in accessDeniedDiagnostics.AccessDeniedInvestigationErrors)
        {
            errors.Add("HP WMI access denied diagnostics: " + error);
        }

        var cimReadiness = HpCimReadinessProbe.Probe();
        foreach (string error in cimReadiness.CimErrors)
        {
            errors.Add("HP CIM readiness probe: " + error);
        }

        var invocationClient = new HpWmiInvocationClient(global::Logger.WriteLine);
        var invocationSandboxStatus = invocationClient.ValidateCatalog(
            hpWmiSnapshot,
            HpBiosWmiCommandCatalog.Definitions);
        var systemDesignDataDryRun = invocationClient.DryRun(
            "SystemDesignData",
            hpWmiSnapshot,
            HpBiosWmiCommandCatalog.Definitions);
        bool systemDesignDataInvocationAllowed =
            global::AppConfig.IsHpVictusHardwareMode() &&
            global::AppConfig.IsHpWmiReadOnlyTestMode();
        var systemDesignDataInvocation = TryInvokeSystemDesignData(
            invocationClient,
            hpWmiSnapshot,
            HpBiosWmiCommandCatalog.Definitions,
            systemDesignDataInvocationAllowed);

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
            hpWmiSnapshot.RootWmiAvailability,
            hpWmiSnapshot.HpqBIntMAvailability,
            hpWmiSnapshot.HpqBDataInAvailability,
            hpWmiSnapshot.HpqBIntMMethodNames,
            hpWmiSnapshot.HpqBDataInMethodNames,
            hpWmiSnapshot.Errors,
            invocationSandboxStatus.InvocationSandboxAvailable,
            invocationSandboxStatus.SafeReadOnlyCommandCount,
            invocationSandboxStatus.RejectedCommandCount,
            invocationSandboxStatus.Errors,
            systemDesignDataDryRun.Status,
            systemDesignDataDryRun.Success && !systemDesignDataDryRun.Invoked,
            systemDesignDataDryRun.Errors,
            systemDesignDataInvocationAllowed,
            systemDesignDataInvocation.Invoked,
            systemDesignDataInvocation.Success,
            systemDesignDataInvocation.ReturnedByteCount ?? 0,
            FirstNonEmpty(systemDesignDataInvocation.Errors),
            accessDeniedDiagnostics.ProcessElevated,
            accessDeniedDiagnostics.WindowsIdentitySummary,
            accessDeniedDiagnostics.WmiNamespaceReadable,
            accessDeniedDiagnostics.HpBIntMClassReadable,
            accessDeniedDiagnostics.HpBIntMMethodMetadataReadable,
            accessDeniedDiagnostics.HpRelatedServices,
            accessDeniedDiagnostics.AccessDeniedInvestigationErrors,
            cimReadiness.CimAvailable,
            cimReadiness.CimRootWmiReachable,
            cimReadiness.CimHpBIntMAvailable,
            cimReadiness.CimHpBIntMMethodMetadataReadable,
            cimReadiness.CimErrors,
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
            snapshot.HpqBIntMMethodNames,
            snapshot.HpqBDataInMethodNames,
            snapshot.HpWmiReadOnlyClientErrors,
            snapshot.InvocationSandboxAvailable,
            snapshot.SafeReadOnlyCommandCount,
            snapshot.RejectedCommandCount,
            snapshot.InvocationSandboxErrors,
            snapshot.SystemDesignDataDryRunStatus,
            snapshot.SystemDesignDataDryRunReady,
            snapshot.SystemDesignDataDryRunErrors,
            snapshot.SystemDesignDataInvocationAllowed,
            snapshot.SystemDesignDataInvocationAttempted,
            snapshot.SystemDesignDataInvocationSucceeded,
            snapshot.SystemDesignDataReturnedByteCount,
            snapshot.SystemDesignDataInvocationError,
            snapshot.ProcessElevated,
            snapshot.WindowsIdentitySummary,
            snapshot.WmiNamespaceReadable,
            snapshot.HpBIntMClassReadable,
            snapshot.HpBIntMMethodMetadataReadable,
            snapshot.HpRelatedServices,
            snapshot.AccessDeniedInvestigationErrors,
            snapshot.CimAvailable,
            snapshot.CimRootWmiReachable,
            snapshot.CimHpBIntMAvailable,
            snapshot.CimHpBIntMMethodMetadataReadable,
            snapshot.CimErrors,
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

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (string? value in values)
        {
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        }

        return string.Empty;
    }

    private static HpWmiInvocationResult TryInvokeSystemDesignData(
        HpWmiInvocationClient invocationClient,
        HpWmiReadOnlySnapshot hpWmiSnapshot,
        IEnumerable<HpBiosWmiCommandDefinition> definitions,
        bool invocationAllowed)
    {
        HpBiosWmiCommandDefinition? definition = definitions.FirstOrDefault(candidate =>
            string.Equals(candidate.Name, "SystemDesignData", StringComparison.OrdinalIgnoreCase));

        if (definition is null)
        {
            return HpWmiInvocationResult.Rejected("SystemDesignData", "command definition not found");
        }

        return invocationClient.TryInvoke(
            new HpWmiInvocationRequest(
                definition,
                global::AppConfig.IsHpVictusHardwareMode(),
                invocationAllowed),
            hpWmiSnapshot);
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
        string[] HpqBIntMMethodNames,
        string[] HpqBDataInMethodNames,
        string[] HpWmiReadOnlyClientErrors,
        bool InvocationSandboxAvailable,
        int SafeReadOnlyCommandCount,
        int RejectedCommandCount,
        string[] InvocationSandboxErrors,
        string SystemDesignDataDryRunStatus,
        bool SystemDesignDataDryRunReady,
        string[] SystemDesignDataDryRunErrors,
        bool SystemDesignDataInvocationAllowed,
        bool SystemDesignDataInvocationAttempted,
        bool SystemDesignDataInvocationSucceeded,
        int SystemDesignDataReturnedByteCount,
        string SystemDesignDataInvocationError,
        bool ProcessElevated,
        string WindowsIdentitySummary,
        bool WmiNamespaceReadable,
        bool HpBIntMClassReadable,
        bool HpBIntMMethodMetadataReadable,
        HpRelatedServiceSnapshot[] HpRelatedServices,
        string[] AccessDeniedInvestigationErrors,
        bool CimAvailable,
        bool CimRootWmiReachable,
        bool CimHpBIntMAvailable,
        bool CimHpBIntMMethodMetadataReadable,
        string[] CimErrors,
        bool LooksLikeHp,
        bool LooksLikeVictus,
        string[] ProbeErrors);
}
