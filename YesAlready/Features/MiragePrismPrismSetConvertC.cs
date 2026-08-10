namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.MiragePrismPrismSetConvertC), BotherCategory.Glamour, "Confirm glamour outfit conversion.")]
public class MiragePrismPrismSetConvertC : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 0);
}
