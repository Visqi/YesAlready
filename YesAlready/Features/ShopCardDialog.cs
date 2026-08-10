namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class ShopCardDialog : AddonFeature
{
    protected override bool IsEnabled() => C.ShopCardDialog;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = addonInfo.GetAddon<AddonShopCardDialog>();
        addon->CardQuantityInput->SetValue(addon->CardQuantityInput->Data.Max);
        addon->GetComponentButtonById(16)->Click(); // SellButton
    }
}
