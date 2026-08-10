namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class RetainerTaskResult : AddonFeature
{
    protected override bool IsEnabled() => C.RetainerTaskResultEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = addonInfo.GetAddon<AddonRetainerTaskResult>();
        if (addon->ResultMode == 2) // 2 == recall
            return;

        Service.TaskManager.Enqueue(() => addon->ReassignButton->IsEnabled);
        Service.TaskManager.Enqueue(() => addon->ReassignButton->Click());
    }
}
