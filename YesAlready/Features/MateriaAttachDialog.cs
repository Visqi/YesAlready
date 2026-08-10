using Dalamud.Game.ClientState.Conditions;
using AddonMateriaAttachDialog = FFXIVClientStructs.FFXIV.Client.UI.AddonMateriaAttachDialog;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.MaterialAttachDialogEnabled), BotherCategory.Melding, "Remove the materia melding confirmation menu.")]
internal class MateriaAttachDialog : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = addonInfo.GetAddon<AddonMateriaAttachDialog>();
        var successRate = addon->TypedAtkValues->SuccessRate.Int;
        if (C.OnlyMeldWhenGuaranteed && successRate < 100)
        {
            PluginLog.Debug($"Success rate {successRate} less than 100%, aborting meld.");
            return;
        }

        Service.TaskManager.Enqueue(() => Svc.Condition[ConditionFlag.MeldingMateria]);
        Service.TaskManager.Enqueue(() => addon->GetComponentButtonById(35)->Click()); // MeldButton
    }
}
