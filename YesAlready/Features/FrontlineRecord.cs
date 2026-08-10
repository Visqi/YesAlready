namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.FrontlineRecordQuit), BotherCategory.PvP, "Automatically leave the Frontline match when the results appear.")]
internal class FrontlineRecord : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, -1);
}
