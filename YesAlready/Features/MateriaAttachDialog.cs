using Dalamud.Game.ClientState.Conditions;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class MateriaAttachDialog : AddonFeature
{
    protected override bool IsEnabled() => C.MaterialAttachDialogEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = (FFXIVClientStructs.FFXIV.Client.UI.AddonMateriaAttachDialog*)atk;
        var successRate = addon->TypedAtkValues->SuccessRate.Int;
        if (C.OnlyMeldWhenGuaranteed && successRate < 100)
        {
            PluginLog.Debug($"Success rate {successRate} less than 100%, aborting meld.");
            return;
        }

        Service.TaskManager.Enqueue(() => Svc.Condition[ConditionFlag.MeldingMateria]);
        Service.TaskManager.Enqueue(() => atk->GetComponentButtonById(35)->Click()); // MeldButton
    }
}
