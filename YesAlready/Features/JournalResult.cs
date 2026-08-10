namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.JournalResultCompleteEnabled), BotherCategory.Other, "Automatically confirm quest reward acceptance when there is nothing to choose.")]
internal class JournalResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AddonJournalResult>()->CompleteButton->Click();
}
