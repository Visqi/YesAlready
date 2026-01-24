namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class JournalAccept : AddonFeature
{
    protected override bool IsEnabled() => C.JournalAcceptAccept;
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk) => new AddonMaster.JournalAccept(atk).Accept();
}
