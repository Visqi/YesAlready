namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[AddonFeature(AddonEvent.PostUpdate)]
internal class HWDLottery : AddonFeature
{
    protected override bool IsEnabled() => C.KupoOfFortune;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = (AddonHWDLottery*)atk;
        switch (eventType)
        {
            case AddonEvent.PostSetup:
                Callback.Fire(atk, true, 0, 1);
                break;
            case AddonEvent.PostUpdate:
                if (addon->Stage == 3 && addon->CloseButton != null && addon->CloseButton->IsEnabled)
                    addon->CloseButton->Click();
                break;
        }
    }
}
