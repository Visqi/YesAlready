namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class ContentsFinderConfirm : AddonFeature
{
    protected override bool IsEnabled() => C.ContentsFinderConfirmEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        addonInfo.GetAddon<AddonContentsFinderConfirm>()->CommenceButton->Click();

        if (C.ContentsFinderOneTimeConfirmEnabled)
        {
            C.ContentsFinderConfirmEnabled = false;
            C.ContentsFinderOneTimeConfirmEnabled = false;
            C.Save();
        }
    }
}
