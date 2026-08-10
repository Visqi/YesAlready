namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.MiragePrismExecuteCast), BotherCategory.Glamour, "Automatically cast glamours when using Glamour Prisms.")]
internal class MiragePrismExecute : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AtkUnitBase>()->GetComponentButtonById(23)->Click(); // CastButton
}
