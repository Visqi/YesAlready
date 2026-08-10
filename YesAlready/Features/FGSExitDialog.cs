namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.FallGuysExitConfirm), BotherCategory.Minigames, "Automatically confirm the exit prompt when leaving Blunderville.")]
internal class FGSExitDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 0);
}
