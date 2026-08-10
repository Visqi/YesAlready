namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.FallGuysRegisterConfirm), BotherCategory.Minigames, "Automatically register for Blunderville when speaking with the Blunderville Registrar.")]
internal class FGSEnterDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 0);
}
