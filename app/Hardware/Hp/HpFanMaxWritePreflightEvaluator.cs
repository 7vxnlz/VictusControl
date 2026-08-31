namespace GHelper.Hardware.Hp;

public static class HpFanMaxWritePreflightEvaluator
{
    public static HpFanWritePreflightResult Evaluate(
        HpFanMaxWritePreflightRequest request,
        HpFanWriteSafetyPolicy? policy = null)
    {
        policy ??= new HpFanWriteSafetyPolicy();
        List<HpFanWriteAbortReason> abortReasons = [];

        if (request.RequestedCommandName != HpFanMaxWriteExperimentPlan.CommandName ||
            request.RequestedCommandId != HpFanMaxWriteExperimentPlan.CommandId)
        {
            abortReasons.Add(HpFanWriteAbortReason.CommandNotSetFanMax);
        }

        if (policy.RequiredFlags.Any(flag => !request.PresentFlags.Contains(flag, StringComparer.Ordinal)))
        {
            abortReasons.Add(HpFanWriteAbortReason.MissingRequiredRuntimeFlag);
        }

        if (policy.RequiresAdministrator && !request.IsAdministrator)
        {
            abortReasons.Add(HpFanWriteAbortReason.AdministratorRequired);
        }

        if (policy.RequiresReadbackBeforeWrite && !request.HasSuccessfulPreReadFanMaxGet)
        {
            abortReasons.Add(HpFanWriteAbortReason.PreWriteReadbackRequired);
        }

        if (request.CurrentMaxFanEnabled is null)
        {
            abortReasons.Add(HpFanWriteAbortReason.CurrentMaxFanStateUnknown);
        }
        else if (request.CurrentMaxFanEnabled.Value)
        {
            abortReasons.Add(HpFanWriteAbortReason.BaselineMaxFanMustBeDisabled);
        }

        if (request.Plan.TargetState is null)
        {
            abortReasons.Add(HpFanWriteAbortReason.WriteTargetStateRequired);
        }

        if (policy.RequiresReadbackAfterWrite && !request.HasPostWriteReadbackPlan)
        {
            abortReasons.Add(HpFanWriteAbortReason.PostWriteReadbackRequired);
        }

        if (policy.RequiresVerifiedRestore &&
            request.Plan.RestoreTargetState != HpFanMaxTargetState.RestoreDisableMaxFan)
        {
            abortReasons.Add(HpFanWriteAbortReason.RestorePlanRequired);
        }

        return new HpFanWritePreflightResult
        {
            IsAllowed = abortReasons.Count == 0,
            AbortReasons = abortReasons
        };
    }
}
