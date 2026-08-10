namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.MKSRecordQuit), BotherCategory.PvP, "Automatically leave the Crystalline Conflict match when the results appear.")]
internal class MKSRecord : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, -1);
}
