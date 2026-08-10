namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostDraw)]
[Bother(nameof(Configuration.RetainerTransferListConfirm), BotherCategory.Retainers, "Skip the confirmation in the RetainerItemTransferList window to entrust all items to the retainer.")]
internal class RetainerItemTransferList : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AddonRetainerItemTransferList>()->ConfirmButton->Click();
}
