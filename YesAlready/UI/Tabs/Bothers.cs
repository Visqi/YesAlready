using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YesAlready.UI.Tabs;

public static class Bothers
{
    private static readonly string[] hotkeyChoices =
    [
        "None",
        "Control",
        "Alt",
        "Shift",
    ];

    private static readonly VirtualKey[] hotkeyValues =
    [
        VirtualKey.NO_KEY,
        VirtualKey.CONTROL,
        VirtualKey.MENU,
        VirtualKey.SHIFT,
    ];

    private static readonly (BotherCategory Category, string Header)[] CategoryHeaders =
    [
        (BotherCategory.Desynthesis, "Desynthesis and Aetherial Reduction"),
        (BotherCategory.Melding, "Melding"),
        (BotherCategory.Retainers, "Retainers and Submersibles"),
        (BotherCategory.Duties, "Duties"),
        (BotherCategory.PvP, "PvP"),
        (BotherCategory.Minigames, "Minigames and Special Events"),
        (BotherCategory.Shops, "Shops"),
        (BotherCategory.Glamour, "Glamour"),
        (BotherCategory.Other, "Other"),
        (BotherCategory.Forays, "Forays"),
    ];

    public static void Draw()
    {
        using var tab = ImRaii.TabItem("Bothers");
        if (!tab) return;
        using var idScope = ImRaii.PushId("BothersOptions");
        using var child = ImRaii.Child("BothersContent", System.Numerics.Vector2.Zero);
        if (!child) return;

        DrawHotkeys();

        var bothersByCategory = FeatureRegistry.Get().GetBothers()
            .GroupBy(x => x.Bother.Category)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (category, header) in CategoryHeaders)
        {
            if (!bothersByCategory.TryGetValue(category, out var bothers) || bothers.Count == 0)
                continue;

            if (!ImGui.CollapsingHeader(header))
                continue;

            foreach (var (feature, bother) in bothers)
                DrawBother(feature, bother);
        }
    }

    private static void DrawHotkeys()
    {
        if (!ImGui.CollapsingHeader("Hotkey Settings"))
            return;

        if (!hotkeyValues.Contains(C.DisableKey))
        {
            C.DisableKey = VirtualKey.NO_KEY;
            C.Save();
        }

        var disableHotkeyIndex = Array.IndexOf(hotkeyValues, C.DisableKey);

        ImGui.SetNextItemWidth(85);
        if (ImGui.Combo("Disable Hotkey", ref disableHotkeyIndex, hotkeyChoices, hotkeyChoices.Length))
        {
            C.DisableKey = hotkeyValues[disableHotkeyIndex];
            C.Save();
        }

        ImGuiX.IndentedTextColored("While this key is held, the plugin is disabled.");

        if (!hotkeyValues.Contains(C.ForcedYesKey))
        {
            C.ForcedYesKey = VirtualKey.NO_KEY;
            C.Save();
        }

        var forcedYesHotkeyIndex = Array.IndexOf(hotkeyValues, C.ForcedYesKey);

        ImGui.SetNextItemWidth(85);
        if (ImGui.Combo("Forced Yes Hotkey", ref forcedYesHotkeyIndex, hotkeyChoices, hotkeyChoices.Length))
        {
            C.ForcedYesKey = hotkeyValues[forcedYesHotkeyIndex];
            C.Save();
        }

        ImGui.SameLine();
        var separateForcedKeys = C.SeparateForcedKeys;
        if (ImGui.Checkbox("Separate Yes/Talk", ref separateForcedKeys))
        {
            C.SeparateForcedKeys = separateForcedKeys;
            C.Save();
        }

        if (C.SeparateForcedKeys)
        {
            var forcedTalkHotkeyIndex = Array.IndexOf(hotkeyValues, C.ForcedTalkKey);
            ImGui.SetNextItemWidth(85);
            if (ImGui.Combo("Forced Talk Hotkey", ref forcedTalkHotkeyIndex, hotkeyChoices, hotkeyChoices.Length))
            {
                C.ForcedTalkKey = hotkeyValues[forcedTalkHotkeyIndex];
                C.Save();
            }
        }

        ImGuiX.IndentedTextColored("2. While this key is held, any Yes/No prompt will always default to yes, and all talk dialogue will be skipped. Be careful.");
    }

    private static void DrawBother(AddonFeature feature, BotherAttribute bother)
    {
        if (bother.RequiresEnabledProperty is { } required
            && !GetConfigBool(required))
            return;

        using var indent = bother.RequiresEnabledProperty is not null ? ImRaii.PushIndent() : null;
        using var id = ImRaii.PushId($"{feature.Key}_{bother.ConfigProperty}");

        var value = GetConfigBool(bother.ConfigProperty);
        if (ImGui.Checkbox(bother.Label is string label ? $"{feature.Key} - {label}" : feature.Key, ref value))
        {
            SetConfigBool(bother.ConfigProperty, value);

            if (value && bother.MutuallyExclusiveWith is { } exclusive)
                SetConfigBool(exclusive, false);

            // ContentsFinderConfirm / OneTimeConfirm coupling
            if (bother.ConfigProperty == nameof(Configuration.ContentsFinderConfirmEnabled) && !value)
                SetConfigBool(nameof(Configuration.ContentsFinderOneTimeConfirmEnabled), false);
            else if (bother.ConfigProperty == nameof(Configuration.ContentsFinderOneTimeConfirmEnabled) && value)
                SetConfigBool(nameof(Configuration.ContentsFinderConfirmEnabled), true);

            C.Save();
        }

        ImGuiX.IndentedTextColored(bother.Description);

        if (bother.ConfigProperty == nameof(Configuration.ItemInspectionResultEnabled) && value)
        {
            using var rateIndent = ImRaii.PushIndent();
            var rateLimit = C.ItemInspectionResultRateLimiter;
            if (ImGui.InputInt("##ItemInspectionRateLimiter", ref rateLimit))
            {
                C.ItemInspectionResultRateLimiter = rateLimit;
                C.Save();
            }
            ImGuiX.IndentedTextColored("Rate limiter (pause after N items, 0 to disable).");
        }
    }

    private static bool GetConfigBool(string propertyName)
    {
        var prop = typeof(Configuration).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Configuration has no property '{propertyName}'.");
        return (bool)prop.GetValue(C)!;
    }

    private static void SetConfigBool(string propertyName, bool value)
    {
        var prop = typeof(Configuration).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Configuration has no property '{propertyName}'.");
        prop.SetValue(C, value);
    }
}
