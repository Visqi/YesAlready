namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class MiragePrismRemove : AddonFeature
{
    protected override bool IsEnabled() => C.MiragePrismRemoveDispel;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
        => atk->GetComponentButtonById(14)->Click(); // DispelButton
}
