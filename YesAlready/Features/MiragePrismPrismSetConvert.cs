using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Linq;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostRefresh)]
public class MiragePrismPrismSetConvert : AddonFeature
{
    private enum ItemFlag : uint
    {
        Missing = 0,
        Unfilled = 2,
        Filled = 3,
        AlreadyInOutfit = 6,
    }

    protected override bool IsEnabled() => C.MiragePrismPrismSetConvert;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        if (!atk->IsAddonReady()) return;

        var addon = (AddonMiragePrismPrismSetConvert*)atk;
        if (addon->AlreadyInDresserText != null && addon->AlreadyInDresserText->AtkResNode.IsVisible())
        {
            Svc.Chat.PrintPluginMessage($"Outfit already in dresser");
            return;
        }

        var itemCount = (int)AgentMiragePrismPrismSetConvert.Instance()->Data->NumItemsInSet;
        var flags = new ItemFlag[itemCount];
        var iconIds = new uint[itemCount];
        for (var i = 0; i < itemCount; i++)
        {
            flags[i] = (ItemFlag)addon->TypedAtkValues->Items[i].Flag.UInt;
            iconIds[i] = addon->TypedAtkValues->Items[i].ItemIconId.UInt;
        }

        if (flags.Any(f => f == ItemFlag.Missing) && !C.AllowPartialFilling)
            return;

        for (var i = 0; i < itemCount; i++)
        {
            if (flags[i] is not ItemFlag.Unfilled)
                continue;

            var s = i;
            var iconId = iconIds[i];
            Service.TaskManager.Enqueue(() => TryHandOver(addon, s, iconId), $"HandInSlot{s}");
            Service.TaskManager.Enqueue(() => (ItemFlag)addon->TypedAtkValues->Items[s].Flag.UInt == ItemFlag.Filled);
        }

        Service.TaskManager.Enqueue(() => addon->StoreAsGlamourButton->Click());
    }

    private static unsafe bool? TryHandOver(AddonMiragePrismPrismSetConvert* addon, int slot, uint itemIconId)
    {
        var flag = (ItemFlag)addon->TypedAtkValues->Items[slot].Flag.UInt;
        if (flag is ItemFlag.Filled or ItemFlag.AlreadyInOutfit)
            return true;

        var contextMenu = (AtkUnitBase*)Svc.GameGui.GetAddonByName("ContextIconMenu", 1).Address;
        if (contextMenu is null || !contextMenu->IsVisible)
        {
            Callback.Fire(&addon->AtkUnitBase, true, 13, slot);
            return false;
        }

        Callback.Fire(contextMenu, true, 0, 0, itemIconId, 0u, 0);
        PluginLog.Debug($"Filled slot {slot}");
        return true;
    }
}
