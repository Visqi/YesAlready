namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.JournalAcceptAccept), BotherCategory.Other, "Automatically accept quests.")]
internal class JournalAccept : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AddonJournalAccept>()->AcceptButton->Click();
}
