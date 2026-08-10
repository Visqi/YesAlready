using Dalamud.Game.Inventory;
using Dalamud.Plugin.Services;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;
using System;
using System.Collections.Generic;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostUpdate)]
internal class SatisfactionSupply : AddonFeature
{
    protected override bool IsEnabled() => C.CustomDeliveries;

    private static bool Disabled;
    private static List<int> SlotsFilled { get; set; } = [];
    private static ulong RequestAllow;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        if (Disabled || !atk->IsAddonReady()) return;
        var addon = (AddonSatisfactionSupply*)atk;
        var values = addon->TypedAtkValues;
        int[] quantities = [values->DoHOwnedQuantity.Int, values->MinBotOwnedQuantity.Int, values->FshOwnedQuantity.Int];

        foreach (var (value, index) in quantities.WithIndex())
        {
            if (value != 0 && !GenericHelpers.TryGetAddonByName<AtkUnitBase>("Request", out var _))
            {
                if (WillItemOvercap(AgentSatisfactionSupply.Instance()->Items[index], Log))
                {
                    Svc.Chat.PrintPluginMessage("Further turn in will overcap scrips.");
                    Disabled = true;
                    return;
                }
                Log($"Turning in item #{AgentSatisfactionSupply.Instance()->Items[index].Id}");
                Callback.Fire(atk, false, 1, index);
            }
        }
    }

    public override void Enable()
    {
        base.Enable();
        Svc.Framework.Update += RequestFill;
        Svc.Framework.Update += RequestComplete;
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "SatisfactionSupply", Reset);
    }

    public override void Disable()
    {
        base.Disable();
        Svc.Framework.Update -= RequestFill;
        Svc.Framework.Update -= RequestComplete;
        Svc.AddonLifecycle.UnregisterListener(Reset);
    }

    private void Reset(AddonEvent type, AddonArgs args) => Disabled = false;

    private static unsafe void RequestFill(IFramework framework)
    {
        if (!P.Active || !C.CustomDeliveries || !GenericHelpers.TryGetAddonByName<AddonRequest>("SatisfactionSupply", out var _))
            return;

        if (GenericHelpers.TryGetAddonByName<AddonRequest>("Request", out var addon) && ((AtkUnitBase*)addon)->IsAddonReady())
        {
            for (var i = 1; i <= addon->EntryCount; i++)
            {
                if (SlotsFilled.Contains(addon->EntryCount))
                {
                    Service.TaskManager.Abort();
                    return;
                }
                if (SlotsFilled.Contains(i)) return;
                var val = i;
                Service.TaskManager.Enqueue(() => TryFillSlot(val));
            }
        }
        else
        {
            SlotsFilled.Clear();
            Service.TaskManager.Abort();
        }
    }

    private static unsafe bool? TryFillSlot(int i)
    {
        if (SlotsFilled.Contains(i)) return true;

        var agent = AgentNpcTrade.Instance();
        if (agent == null || !agent->IsAgentActive())
            return false;

        var slot = (ushort)(i - 1);
        if (agent->SelectedTurnInSlot >= 0 && agent->SelectedTurnInSlot != slot)
            return false;

        if (agent->SelectedTurnInSlot != slot)
        {
            agent->SelectTurnInSlot(slot);
            return false;
        }

        if (agent->SelectedTurnInSlotItemOptions <= 0)
            return false;

        var res = new AtkValue();
        Span<AtkValue> param = stackalloc AtkValue[4];
        param[0].SetInt(0);
        param[1].SetInt(0);
        agent->ReceiveEvent(&res, param.GetPointer(0), 4, 1);

        PluginLog.Debug($"Filled slot {i}");
        SlotsFilled.Add(i);
        return true;
    }

    private static unsafe void RequestComplete(IFramework framework)
    {
        if (!P.Active || !C.CustomDeliveries || !GenericHelpers.TryGetAddonByName<AddonRequest>("SatisfactionSupply", out var _))
            return;

        if (GenericHelpers.TryGetAddonByName<AddonRequest>("Request", out var addon) && ((AtkUnitBase*)addon)->IsAddonReady())
        {
            if (RequestAllow == 0)
                RequestAllow = Svc.PluginInterface.UiBuilder.FrameCount + 4;

            if (Svc.PluginInterface.UiBuilder.FrameCount < RequestAllow) return;

            var handOver = addon->HandOverButton;
            if (handOver->IsEnabled && EzThrottler.Throttle("Handin"))
            {
                PluginLog.Debug("Handing over request");
                handOver->Click();
            }
        }
        else
            RequestAllow = 0;
    }

    private static unsafe bool WillItemOvercap(AgentSatisfactionSupply.ItemInfo item, Action<string> log)
    {
        if (GetItem(item.Id) is { SpiritbondOrCollectability: var collectability })
        {
            log($"Checking overcap for item #{item.Id} with collectability {collectability}");
            if (collectability > item.Collectability3)
            {
                log($"Item #{item.Id} [{item.Reward1Quantity[2]} > {CurrencyManager.Instance()->GetItemCountRemaining(item.Reward1Id)} || {item.Reward2Quantity[2]} > {CurrencyManager.Instance()->GetItemCountRemaining(item.Reward2Id)}]");
                return CurrencyManager.Instance()->GetItemCountRemaining(item.Reward1Id) < item.Reward1Quantity[2] || CurrencyManager.Instance()->GetItemCountRemaining(item.Reward2Id) < item.Reward2Quantity[2];
            }
            if (collectability > item.Collectability2)
            {
                log($"Item #{item.Id} [{item.Reward1Quantity[1]} > {CurrencyManager.Instance()->GetItemCountRemaining(item.Reward1Id)} || {item.Reward2Quantity[1]} > {CurrencyManager.Instance()->GetItemCountRemaining(item.Reward2Id)}]");
                return CurrencyManager.Instance()->GetItemCountRemaining(item.Reward1Id) < item.Reward1Quantity[1] || CurrencyManager.Instance()->GetItemCountRemaining(item.Reward2Id) < item.Reward2Quantity[1];
            }
            if (collectability > item.Collectability1)
            {
                log($"Item #{item.Id} [{item.Reward1Quantity[0]} > {CurrencyManager.Instance()->GetItemCountRemaining(item.Reward1Id)} || {item.Reward2Quantity[0]} > {CurrencyManager.Instance()->GetItemCountRemaining(item.Reward2Id)}]");
                return CurrencyManager.Instance()->GetItemCountRemaining(item.Reward1Id) < item.Reward1Quantity[0] || CurrencyManager.Instance()->GetItemCountRemaining(item.Reward2Id) < item.Reward2Quantity[0];
            }
        }
        throw new Exception($"Failed to find item [{item.Id}] in inventory");
    }

    private static GameInventoryItem? GetItem(uint itemId)
    {
        IEnumerable<GameInventoryType> types = [GameInventoryType.Inventory1, GameInventoryType.Inventory2, GameInventoryType.Inventory3, GameInventoryType.Inventory4];
        foreach (var type in types)
        {
            var items = Svc.GameInventory.GetInventoryItems(type);
            foreach (var item in items)
                if (item.BaseItemId == itemId)
                    return item;
        }
        return null;
    }
}
