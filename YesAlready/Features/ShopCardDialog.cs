namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.ShopCardDialog), BotherCategory.Minigames, "Automatically confirm selling Triple Triad cards in the saucer.")]
internal class ShopCardDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = addonInfo.GetAddon<AddonShopCardDialog>();
        addon->CardQuantityInput->SetValue(addon->CardQuantityInput->Data.Max);
        addon->GetComponentButtonById(16)->Click(); // SellButton
    }
}
