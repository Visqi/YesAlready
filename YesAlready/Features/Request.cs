using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;
using System;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal unsafe class Request : AddonFeature
{
    private static readonly delegate* unmanaged<NpcTrade*, byte> CanSatisfyRequests = (delegate* unmanaged<NpcTrade*, byte>)Svc.SigScanner.ScanText("0F B6 91 ?? ?? ?? ?? C6 81 ?? ?? ?? ?? ?? 48 83 C1 08");
    private static readonly delegate* unmanaged<AgentNpcTrade*, ushort, uint, uint, void> SelectTurnInSlot = (delegate* unmanaged<AgentNpcTrade*, ushort, uint, uint, void>)Svc.SigScanner.ScanText("44 89 44 24 ?? 53 41 54 41 56");

    protected override bool IsEnabled() => C.RequestFill;

    protected override void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var npcTrade = &UIState.Instance()->NpcTrade;
        var agent = AgentNpcTrade.Instance();
        if (agent == null || !agent->IsAgentActive())
        {
            Log($"AgentNpcTrade null or inactive");
            return;
        }

        if (npcTrade->Requests.Count == 0 || CanSatisfyRequests(npcTrade) == 0)
        {
            Log($"No requests or cannot satisfy requests");
            return;
        }

        var res = new AtkValue();
        Span<AtkValue> param = stackalloc AtkValue[4];

        for (var slot = 0; slot < npcTrade->Requests.Count; slot++)
        {
            if (agent->SelectedTurnInSlot >= 0)
                return; // mid-select

            SelectTurnInSlot(agent, (ushort)slot, 0, 0);
            if (agent->SelectedTurnInSlot != slot || agent->SelectedTurnInSlotItemOptions <= 0)
                return;

            param[0].SetInt(0); // confirm
            param[1].SetInt(0); // option 0 (NQ/HQ pick - first matching)
            agent->ReceiveEvent(&res, param.GetPointer(0), 4, 1);
        }

        // commit
        param[0].SetInt(0);
        var addonId = agent->AddonId;
        agent->ReceiveEvent(&res, param.GetPointer(0), 4, 0);
        var addon = RaptureAtkUnitManager.Instance()->GetAddonById((ushort)addonId);
        if (addon != null && addon->IsVisible)
            addon->Close(false);
    }
}
