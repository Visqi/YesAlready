namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
public class MiragePrismPrismSetConvertC : AddonFeature
{
    protected override bool IsEnabled() => C.MiragePrismPrismSetConvertC;
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk) => Callback.Fire(atk, true, 0);
}
