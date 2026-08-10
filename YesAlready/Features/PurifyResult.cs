namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostUpdate)]
internal class PurifyResult : AddonFeature
{
    protected override bool IsEnabled() => C.AetherialReductionResults;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        if (!GenericHelpers.IsAddonReady(atk)) return;

        var addon = (AddonPurifyResult*)atk;
        if (addon->TypedAtkValues->ResultsMode.UInt == 0) // non-zero when done
            return;

        PluginLog.Debug("Closing Purify Results menu");
        Callback.Fire(atk, true, -1);
    }
}
