namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class SalvageDialog : AddonFeature
{
    protected override bool IsEnabled() => C.DesynthDialogEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
        => addonInfo.GetAddon<AddonSalvageDialog>()->DesynthesizeButton->Click();
}
