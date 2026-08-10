using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class ItemInspectionResult : AddonFeature
{
    private int itemInspectionCount = 0;

    protected override bool IsEnabled() => C.ItemInspectionResultEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        var addon = (AddonItemInspectionResult*)atk;
        if (atk->UldManager.NodeListCount < 64) return;

        var nameNode = atk->GetTextNodeById(26);
        var descNode = atk->GetTextNodeById(35);
        if (nameNode == null || descNode == null || !nameNode->AtkResNode.IsVisible() || !descNode->AtkResNode.IsVisible())
            return;

        var values = addon->TypedAtkValues;
        var description = values->Description.Type == AtkValueType.String
            ? MemoryHelper.ReadSeStringNullTerminated((nint)values->Description.String.Value).GetText()
            : descNode->NodeText.GetText();

        if (description.Contains('※') || description.Contains("liées à Garde-la-Reine"))
        {
            var itemName = addon->ShowingAltName != 0 && !addon->ItemNameAlt.IsEmpty
                ? addon->ItemNameAlt.ToString()
                : (!addon->ItemName.IsEmpty ? addon->ItemName.ToString() : nameNode->NodeText.GetText());
            Svc.Chat.PrintPluginMessage(new SeString(new TextPayload("Received: "), new TextPayload(itemName)));
        }

        itemInspectionCount++;
        var rateLimiter = C.ItemInspectionResultRateLimiter;
        if (rateLimiter != 0 && itemInspectionCount % rateLimiter == 0)
        {
            itemInspectionCount = 0;
            Svc.Chat.PrintPluginMessage("Rate limited, pausing item inspection loop.");
            return;
        }

        var nextButton = atk->GetComponentButtonById(74);
        var closeButton = atk->GetComponentButtonById(73);
        if (values->HasNext.Int != 0 && nextButton->IsEnabled)
            nextButton->Click();
        else
            closeButton->Click();
    }
}
