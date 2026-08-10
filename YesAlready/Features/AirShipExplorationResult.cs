namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class AirShipExplorationResult : AddonFeature
{
    protected override bool IsEnabled() => C.AirShipExplorationResultFinalize || C.AirShipExplorationResultRedeploy;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        if (C.AirShipExplorationResultFinalize)
            addonInfo.GetAddon<AddonAirShipExplorationResult>()->FinalizeReportButton->Click();

        if (C.AirShipExplorationResultRedeploy)
            addonInfo.GetAddon<AddonAirShipExplorationResult>()->RedeployButton->Click();
    }
}
