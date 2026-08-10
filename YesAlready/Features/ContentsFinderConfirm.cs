namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.ContentsFinderConfirmEnabled), BotherCategory.Duties, "Automatically commence duties when ready.")]
[Bother(nameof(Configuration.ContentsFinderOneTimeConfirmEnabled), BotherCategory.Duties,
    "Automatically commence duties when ready, but only once. Requires Contents Finder Confirm, and disables both after activation.",
    label: "One Time",
    ContributesToEnable = false)]
internal class ContentsFinderConfirm : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
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
