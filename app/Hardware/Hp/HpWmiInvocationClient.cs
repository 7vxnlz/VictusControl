namespace GHelper.Hardware.Hp;

public sealed class HpWmiInvocationClient
{
    private readonly Action<string>? _log;

    public HpWmiInvocationClient(Action<string>? log = null)
    {
        _log = log;
    }

    public HpWmiInvocationSandboxStatus ValidateCatalog(
        HpWmiReadOnlySnapshot wmiSnapshot,
        IEnumerable<HpBiosWmiCommandDefinition> definitions)
    {
        var errors = new List<string>();
        int safeReadOnlyCommandCount = 0;
        int rejectedCommandCount = 0;
        bool sandboxAvailable = IsRequiredWmiSurfaceAvailable(wmiSnapshot);
        var exposedMethods = new HashSet<string>(wmiSnapshot.HpqBIntMMethodNames, StringComparer.OrdinalIgnoreCase);

        foreach (HpBiosWmiCommandDefinition definition in definitions)
        {
            string? rejectionReason = GetRejectionReason(definition, sandboxAvailable, exposedMethods);
            if (rejectionReason is null)
            {
                safeReadOnlyCommandCount++;
                continue;
            }

            rejectedCommandCount++;
            _log?.Invoke($"HP WMI invocation sandbox rejected '{definition.Name}': {rejectionReason}");
        }

        if (!sandboxAvailable)
        {
            errors.Add("Required HP WMI surface is unavailable.");
        }

        errors.AddRange(wmiSnapshot.Errors);

        return new HpWmiInvocationSandboxStatus(
            sandboxAvailable,
            safeReadOnlyCommandCount,
            rejectedCommandCount,
            errors.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }

    public HpWmiInvocationResult TryInvoke(HpWmiInvocationRequest request, HpWmiReadOnlySnapshot wmiSnapshot)
    {
        bool sandboxAvailable = IsRequiredWmiSurfaceAvailable(wmiSnapshot);
        var exposedMethods = new HashSet<string>(wmiSnapshot.HpqBIntMMethodNames, StringComparer.OrdinalIgnoreCase);
        string? rejectionReason = GetRejectionReason(request.CommandDefinition, sandboxAvailable, exposedMethods);

        if (rejectionReason is not null)
        {
            _log?.Invoke($"HP WMI invocation sandbox rejected '{request.CommandDefinition.Name}': {rejectionReason}");
            return HpWmiInvocationResult.Rejected(request.CommandDefinition, rejectionReason);
        }

        return HpWmiInvocationResult.Rejected(
            request.CommandDefinition,
            "HP BIOS WMI invocation is intentionally disabled in this milestone.");
    }

    public HpWmiInvocationResult DryRun(
        string commandName,
        HpWmiReadOnlySnapshot wmiSnapshot,
        IEnumerable<HpBiosWmiCommandDefinition> definitions)
    {
        HpBiosWmiCommandDefinition? definition = definitions.FirstOrDefault(candidate =>
            string.Equals(candidate.Name, commandName, StringComparison.OrdinalIgnoreCase));

        if (definition is null)
        {
            const string reason = "command definition not found";
            _log?.Invoke($"HP WMI invocation sandbox rejected '{commandName}': {reason}");
            return HpWmiInvocationResult.Rejected(commandName, reason);
        }

        return DryRun(new HpWmiInvocationRequest(definition), wmiSnapshot);
    }

    public HpWmiInvocationResult DryRun(HpWmiInvocationRequest request, HpWmiReadOnlySnapshot wmiSnapshot)
    {
        bool sandboxAvailable = IsRequiredWmiSurfaceAvailable(wmiSnapshot);
        var exposedMethods = new HashSet<string>(wmiSnapshot.HpqBIntMMethodNames, StringComparer.OrdinalIgnoreCase);
        string? rejectionReason = GetRejectionReason(request.CommandDefinition, sandboxAvailable, exposedMethods);

        if (rejectionReason is not null)
        {
            _log?.Invoke($"HP WMI invocation sandbox dry-run rejected '{request.CommandDefinition.Name}': {rejectionReason}");
            return HpWmiInvocationResult.Rejected(request.CommandDefinition, rejectionReason);
        }

        _log?.Invoke($"HP WMI invocation sandbox dry-run ready for '{request.CommandDefinition.Name}'. No HP BIOS WMI method was invoked.");
        return HpWmiInvocationResult.DryRunReady(request.CommandDefinition);
    }

    private static bool IsRequiredWmiSurfaceAvailable(HpWmiReadOnlySnapshot wmiSnapshot) =>
        wmiSnapshot.RootWmiAvailability == HpVictusProbeAvailability.Available &&
        wmiSnapshot.HpqBIntMAvailability == HpVictusProbeAvailability.Available &&
        wmiSnapshot.HpqBDataInAvailability == HpVictusProbeAvailability.Available;

    private static string? GetRejectionReason(
        HpBiosWmiCommandDefinition definition,
        bool sandboxAvailable,
        ISet<string> exposedMethods)
    {
        if (!sandboxAvailable)
        {
            return "required HP WMI surface is unavailable";
        }

        if (definition.Access != HpBiosWmiCommandAccess.ReadOnly)
        {
            return "command is not read-only";
        }

        if (definition.Safety != HpBiosWmiCommandSafety.SafeReadOnlyInvocation)
        {
            return "command is not explicitly marked safe for invocation";
        }

        if (definition.ExpectedInputSize < 0 || definition.ExpectedOutputSize < 0)
        {
            return "command has invalid input or output size metadata";
        }

        if (string.IsNullOrWhiteSpace(definition.MethodName) || !definition.MethodName.StartsWith("hpqBIOSInt", StringComparison.Ordinal))
        {
            return "command method name is not an HP BIOS WMI method";
        }

        if (!exposedMethods.Contains(definition.MethodName))
        {
            return "command method is not exposed by hpqBIntM on this machine";
        }

        string expectedMethodName = "hpqBIOSInt" + definition.ExpectedOutputSize.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (!string.Equals(definition.MethodName, expectedMethodName, StringComparison.Ordinal))
        {
            return "command method name does not match expected output size";
        }

        return null;
    }
}
