namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.RetainerTaskAskEnabled), BotherCategory.Retainers, "Skip the confirmation in the final dialog before sending out a retainer.")]
internal class RetainerTaskAsk : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = addonInfo.GetAddon<AddonRetainerTaskAsk>();
        Service.TaskManager.Enqueue(() => addon->AssignButton->IsEnabled);
        Service.TaskManager.Enqueue(() => addon->AssignButton->Click());
    }
}
