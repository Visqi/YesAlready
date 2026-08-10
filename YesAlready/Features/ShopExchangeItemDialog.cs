namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.ShopExchangeItemDialogEnabled), BotherCategory.Shops, "Automatically exchange items/currencies in various shops (e.g., scrip vendors).")]
internal class ShopExchangeItemDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 0);
}
