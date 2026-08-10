namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[AddonFeature(AddonEvent.PostUpdate, "SalvageAutoDialog")]
[Bother(nameof(Configuration.DesynthesisResults), BotherCategory.Desynthesis, "Automatically closes the SalvageResults window when done desynthesizing.")]
internal class SalvageResult : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var atk = addonInfo.GetAddon<AtkUnitBase>();
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
