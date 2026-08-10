namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class MiragePrismExecute : AddonFeature
{
    protected override bool IsEnabled() => C.MiragePrismExecuteCast;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
        => atk->GetComponentButtonById(23)->Click(); // CastButton
}
