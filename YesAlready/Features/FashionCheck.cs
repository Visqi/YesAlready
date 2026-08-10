namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.FashionCheckQuit), BotherCategory.Minigames, "Automatically confirm the Fashion Reports results.")]
internal class FashionCheck : AddonFeature
{
    protected override unsafe bool IsEnabled()
        => base.IsEnabled() && !GenericHelpers.TryGetAddonByName<AtkUnitBase>("ContentsInfo", out var _); // do not fire when the timers window is also open

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, -1);
}
