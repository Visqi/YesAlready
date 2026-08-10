namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class RetainerTaskAsk : AddonFeature
{
    protected override bool IsEnabled() => C.RetainerTaskAskEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = addonInfo.GetAddon<AddonRetainerTaskAsk>();
        Service.TaskManager.Enqueue(() => addon->AssignButton->IsEnabled);
        Service.TaskManager.Enqueue(() => addon->AssignButton->Click());
    }
}
