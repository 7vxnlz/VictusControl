namespace GHelper.Hardware.Hp;

public sealed record HpFanMaxPayloadDescription(
    HpFanMaxTargetState TargetState,
    int ExpectedInputLength,
    byte FirstByteValue,
    int ZeroFilledTrailingByteCount)
{
    public const int ReferenceExpectedInputLength = 4;

    public static HpFanMaxPayloadDescription Describe(HpFanMaxTargetState targetState) =>
        targetState switch
        {
            HpFanMaxTargetState.EnableMaxFan => new(targetState, ReferenceExpectedInputLength, 1, 3),
            HpFanMaxTargetState.RestoreDisableMaxFan => new(targetState, ReferenceExpectedInputLength, 0, 3),
            _ => throw new ArgumentOutOfRangeException(nameof(targetState), targetState, "Only named SetFanMax target states are supported.")
        };
}
