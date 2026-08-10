namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.ChocoboRacingQuit), BotherCategory.Minigames, "Automatically quit Chocobo Racing when the results menu appears.")]
internal class RaceChocoboResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 1);
}
