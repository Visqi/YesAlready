namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostUpdate)]
[Bother(nameof(Configuration.RetainerTransferProgressConfirm), BotherCategory.Retainers, "Automatically closes the RetainerItemTransferProgress window when finished entrusting items.")]
internal class RetainerItemTransferProgress : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        if (addonInfo.GetAddon<AddonRetainerItemTransferProgress>()->TypedAtkValues->Progress.Float < 1f)
            return;

        Log("Closing Entrust Duplicates menu");
        addonInfo.GetAddon<AddonRetainerItemTransferProgress>()->CloseWindowButton->Click();
    }
}
