namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.LordOfVerminionQuit), BotherCategory.Minigames, "Automatically quit Lord of Verminion when the results menu appears.")]
internal class LovmResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, -1);
}
