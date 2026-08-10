namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.GrandCompanySupplyReward), BotherCategory.Other, "Skip the confirmation when submitting Grand Company expert delivery items.")]
internal class GrandCompanySupplyReward : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AddonGrandCompanySupplyReward>()->DeliverButton->Click();
}
