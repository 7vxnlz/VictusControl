using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace GHelper.Hardware.Hp;

public static class HpCimReadinessProbe
{
    private const string BiosNamespace = @"root\wmi";
    private const string BiosDataClassName = "hpqBDataIn";
    private const string BiosMethodClassName = "hpqBIntM";
    private const string BiosMethodInstance = @"ACPI\PNP0C14\0_0";

    public static HpCimReadinessSnapshot Probe()
    {
        var errors = new List<string>();

        bool cimAvailable = IsCimApiAvailable(errors);
        bool rootWmiReachable = false;
        bool hpBIntMAvailable = false;
        bool methodMetadataReadable = false;

        if (cimAvailable)
        {
            try
            {
                using CimSession session = CimSession.Create(null);

                rootWmiReachable = CanReadClass(session, BiosNamespace, BiosDataClassName, errors, "CIM root\\wmi / hpqBDataIn");
                hpBIntMAvailable = CanReadClass(session, BiosNamespace, BiosMethodClassName, errors, "CIM hpqBIntM");
                methodMetadataReadable = CanReadMethodMetadata(session, errors);
                TryResolveBiosMethodInstance(session, errors);
            }
            catch (Exception ex) when (IsCimReadinessException(ex))
            {
                errors.Add("CIM session setup: " + FormatException(ex));
            }
        }

        return new HpCimReadinessSnapshot(
            cimAvailable,
            rootWmiReachable,
            hpBIntMAvailable,
            methodMetadataReadable,
            errors.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }

    private static bool IsCimApiAvailable(List<string> errors)
    {
        try
        {
            return Type.GetType("Microsoft.Management.Infrastructure.CimSession, Microsoft.Management.Infrastructure") is not null;
        }
        catch (Exception ex) when (ex is TypeLoadException or FileLoadException or BadImageFormatException)
        {
            errors.Add("CIM API availability: " + FormatException(ex));
            return false;
        }
    }

    private static bool CanReadClass(CimSession session, string namespaceName, string className, List<string> errors, string label)
    {
        try
        {
            _ = session.GetClass(namespaceName, className);
            return true;
        }
        catch (Exception ex) when (IsCimReadinessException(ex))
        {
            errors.Add(label + ": " + FormatException(ex));
            return false;
        }
    }

    private static bool CanReadMethodMetadata(CimSession session, List<string> errors)
    {
        try
        {
            CimClass methodClass = session.GetClass(BiosNamespace, BiosMethodClassName);
            _ = methodClass.CimClassMethods
                .Select(method => method.Name)
                .Where(name => name.StartsWith("hpqBIOSInt", StringComparison.Ordinal))
                .ToArray();

            return true;
        }
        catch (Exception ex) when (IsCimReadinessException(ex))
        {
            errors.Add("CIM hpqBIntM method metadata: " + FormatException(ex));
            return false;
        }
    }

    private static void TryResolveBiosMethodInstance(CimSession session, List<string> errors)
    {
        try
        {
            using var biosMethods = new CimInstance(BiosMethodClassName, BiosNamespace);
            biosMethods.CimInstanceProperties.Add(CimProperty.Create("InstanceName", BiosMethodInstance, CimFlags.Key));

            using CimInstance? _ = session.GetInstance(BiosNamespace, biosMethods);
        }
        catch (Exception ex) when (IsCimReadinessException(ex))
        {
            errors.Add("CIM hpqBIntM method instance metadata: " + FormatException(ex));
        }
    }

    private static bool IsCimReadinessException(Exception ex) =>
        ex is CimException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException;

    private static string FormatException(Exception ex) => $"{ex.GetType().Name}: {ex.Message}";
}
