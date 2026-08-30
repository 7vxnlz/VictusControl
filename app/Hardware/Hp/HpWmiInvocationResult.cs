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
}

public sealed record HpWmiInvocationSandboxStatus(
    bool InvocationSandboxAvailable,
    int SafeReadOnlyCommandCount,
    int RejectedCommandCount,
    string[] Errors);
