namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.DesynthDialogEnabled), BotherCategory.Desynthesis, "Remove the Desynthesis menu confirmation.")]
internal class SalvageDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AddonSalvageDialog>()->DesynthesizeButton->Click();
}
