using System;
using System.Linq;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class InputNumeric : TextMatchingFeature
{
    protected override unsafe string GetSetLastSeenText(AtkUnitBase* atk)
    {
        var addon = (AddonInputNumeric*)atk;
        var text = addon->TypedAtkValues->PromptText.String.ToString();
        Service.Watcher.LastSeenNumericsText = text;
        return text;
    }

    protected override unsafe object? ShouldProceed(string text, AtkUnitBase* atk)
    {
        var nodes = C.GetAllNodes().OfType<NumericsEntryNode>();
        foreach (var node in nodes)
        {
            if (!node.Enabled || string.IsNullOrEmpty(node.Text))
                continue;

            if (!CheckRestrictions(node))
                continue;

            if (EntryMatchesText(node.Text, text, node.IsTextRegex))
                return node;
        }

        return null;
    }

    protected override unsafe void Proceed(AtkUnitBase* atk, object? matchingNode)
    {
        if (matchingNode is not NumericsEntryNode node) return;

        var addon = (AddonInputNumeric*)atk;
        var min = (uint)addon->NumericInput->Data.Min;
        var max = (uint)addon->NumericInput->Data.Max;

        Log("Selecting ok");
        var value = Math.Clamp(node.IsPercent ? (uint)Math.Ceiling(max * (node.Percentage / 100f)) : (uint)node.Quantity, min, max);
        Callback.Fire(atk, true, (int)value);
    }
}
