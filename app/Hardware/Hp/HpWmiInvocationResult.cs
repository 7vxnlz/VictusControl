namespace GHelper.Hardware.Hp;

public sealed record HpWmiInvocationResult(
    bool Success,
    bool Invoked,
    string CommandName,
    string MethodName,
    string Status,
    string[] Errors)
{
    public static HpWmiInvocationResult Rejected(HpBiosWmiCommandDefinition definition, string reason) =>
        new(false, false, definition.Name, definition.MethodName, "Rejected", [reason]);

    public static HpWmiInvocationResult Rejected(string commandName, string reason) =>
        new(false, false, commandName, string.Empty, "Rejected", [reason]);

    public static HpWmiInvocationResult DryRunReady(HpBiosWmiCommandDefinition definition) =>
        new(true, false, definition.Name, definition.MethodName, "DryRunReady", []);
}

public sealed record HpWmiInvocationSandboxStatus(
    bool InvocationSandboxAvailable,
    int SafeReadOnlyCommandCount,
    int RejectedCommandCount,
    string[] Errors);
