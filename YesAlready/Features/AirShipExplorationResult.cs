namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.AirShipExplorationResultFinalize), BotherCategory.Retainers,
    "Automatically finalize submersible reports when the AirShipExplorationResult window opens.",
    label: "Finalize",
    MutuallyExclusiveWith = nameof(Configuration.AirShipExplorationResultRedeploy))]
[Bother(nameof(Configuration.AirShipExplorationResultRedeploy), BotherCategory.Retainers,
    "Automatically redeploy submersibles when the AirShipExplorationResult window opens.",
    label: "Redeploy",
    MutuallyExclusiveWith = nameof(Configuration.AirShipExplorationResultFinalize))]
internal class AirShipExplorationResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        if (C.AirShipExplorationResultFinalize)
            addonInfo.GetAddon<AddonAirShipExplorationResult>()->FinalizeReportButton->Click();

        if (C.AirShipExplorationResultRedeploy)
            addonInfo.GetAddon<AddonAirShipExplorationResult>()->RedeployButton->Click();
    }
}
