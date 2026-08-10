namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.MateriaRetrieveDialogEnabled), BotherCategory.Melding, "Remove the retrieve materia confirmation.")]
internal class MateriaRetrieveDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => addonInfo.GetAddon<AtkUnitBase>()->GetComponentButtonById(17)->Click(); // BeginButton
}
