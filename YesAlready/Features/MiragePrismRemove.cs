namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.MiragePrismRemoveDispel), BotherCategory.Glamour, "Automatically dispel glamours when using Glamour Dispellers.")]
internal class MiragePrismRemove : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AtkUnitBase>()->GetComponentButtonById(14)->Click(); // DispelButton
}
