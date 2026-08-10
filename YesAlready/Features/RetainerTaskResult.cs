namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.RetainerTaskResultEnabled), BotherCategory.Retainers, "Automatically send a retainer on the same venture as before when receiving an item.")]
internal class RetainerTaskResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = addonInfo.GetAddon<AddonRetainerTaskResult>();
        if (addon->ResultMode == 2) // 2 == recall
            return;

        Service.TaskManager.Enqueue(() => addon->ReassignButton->IsEnabled);
        Service.TaskManager.Enqueue(() => addon->ReassignButton->Click());
    }
}
