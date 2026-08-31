namespace GHelper.Hardware.Hp;

public sealed record HpFanMaxReferenceInputShape(
    int InputLength,
    int ZeroFilledTrailingByteCount);

public sealed record HpFanMaxPayloadDescription(
    HpFanMaxTargetState TargetState,
    byte FirstByteValue,
    IReadOnlyList<HpFanMaxReferenceInputShape> ObservedReferenceInputShapes,
    int? DeviceValidatedInputLength)
{
    public const string ReferenceMethodName = "hpqBIOSInt0";
    public const uint ReferenceCommandValue = 0x20008;
    public const uint ReferenceCommandType = 0x27;
    public const int ReferenceExpectedOutputSize = 0;

    private static readonly IReadOnlyList<HpFanMaxReferenceInputShape> ReferenceInputShapes = Array.AsReadOnly(
    [
        new HpFanMaxReferenceInputShape(InputLength: 4, ZeroFilledTrailingByteCount: 3),
        new HpFanMaxReferenceInputShape(InputLength: 1, ZeroFilledTrailingByteCount: 0)
    ]);

    public static HpFanMaxPayloadDescription Describe(HpFanMaxTargetState targetState) =>
        targetState switch
        {
            HpFanMaxTargetState.EnableMaxFan => new(targetState, 1, ReferenceInputShapes, DeviceValidatedInputLength: null),
            HpFanMaxTargetState.RestoreDisableMaxFan => new(targetState, 0, ReferenceInputShapes, DeviceValidatedInputLength: null),
            _ => throw new ArgumentOutOfRangeException(nameof(targetState), targetState, "Only named SetFanMax target states are supported.")
        };
}
