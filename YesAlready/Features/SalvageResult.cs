namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[AddonFeature(AddonEvent.PostUpdate, "SalvageAutoDialog")]
internal class SalvageResult : AddonFeature
{
    protected override bool IsEnabled() => C.DesynthesisResults;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        if (!atk->IsAddonReady()) return;

        switch (addonInfo.AddonName)
        {
            case "SalvageResult":
                atk->GetComponentButtonById(15)->Click(); // CloseButton
                break;

            case "SalvageAutoDialog":
                var addon = addonInfo.GetAddon<AddonSalvageAutoDialog>();
                if (!addon->IsDesynthesizing)
                    addon->EndDesynthesisButton->Click();
                break;
        }
    }
}
