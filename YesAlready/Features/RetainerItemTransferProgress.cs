namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostUpdate)]
internal class RetainerItemTransferProgress : AddonFeature
{
    protected override bool IsEnabled() => C.RetainerTransferProgressConfirm;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = addonInfo.GetAddon<AddonRetainerItemTransferProgress>();
        if (addon->TypedAtkValues->Progress.Float < 1f)
            return;

        PluginLog.Debug("Closing Entrust Duplicates menu");
        addon->CloseWindowButton->Click();
    }
}
