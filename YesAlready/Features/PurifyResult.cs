namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostUpdate)]
[Bother(nameof(Configuration.AetherialReductionResults), BotherCategory.Desynthesis, "Automatically closes the PurifyResult window when done reducing.")]
internal class PurifyResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = addonInfo.GetAddon<AddonPurifyResult>();
        if (!addon->AtkUnitBase.IsAddonReady()) return;

        if (addon->TypedAtkValues->ResultsMode.UInt == 0) // non-zero when done
            return;

        PluginLog.Debug("Closing Purify Results menu");
        Callback.Fire((AtkUnitBase*)addon, true, -1);
    }
}
