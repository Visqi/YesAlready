using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace YesAlready.Interface;

internal class MateriaBlacklistWindow : Window
{
    private string _search = string.Empty;

    public MateriaBlacklistWindow() : base($"{Name} Materia Blacklist")
    {
        Size = new Vector2(720, 640);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw()
    {
        using var _ = ImRaii.PushColor(ImGuiCol.ResizeGrip, 0);

        var catalog = MateriaCatalog.Get();
        var types = catalog.CurrentTypes;
        var grades = catalog.CurrentGrades;
        var selected = C.TradeMultipleBlacklistItemIds;

        ImGui.Text($"Blacklisted: {selected.Count}");
        ImGui.TextDisabled("Outdated/not shown materia are automatically blacklisted");
        ImGui.Separator();

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        ImGui.InputTextWithHint("##MateriaBlacklistSearch", "Filter by name", ref _search, 128);

        var search = _search.Trim();
        var visible = types.Where(t => search.Length == 0 || t.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        ImGui.Spacing();
        ImGui.Text("Category");
        if (ImGui.Button("Battle"))
            SetMany(visible.Where(t => t.Category == MateriaCategory.Battle).SelectMany(t => t.Grades.Select(g => g.ItemId)), true);
        ImGui.SameLine();
        if (ImGui.Button("DoH"))
            SetMany(visible.Where(t => t.Category == MateriaCategory.DoH).SelectMany(t => t.Grades.Select(g => g.ItemId)), true);
        ImGui.SameLine();
        if (ImGui.Button("DoL"))
            SetMany(visible.Where(t => t.Category == MateriaCategory.DoL).SelectMany(t => t.Grades.Select(g => g.ItemId)), true);
        ImGui.SameLine();
        if (ImGui.Button("Clear All"))
        {
            var currentIds = new HashSet<uint>(types.SelectMany(t => t.Grades.Select(g => g.ItemId)));
            if (selected.RemoveAll(currentIds.Contains) > 0)
                C.Save();
        }

        ImGui.Spacing();
        ImGui.Text("Grade");
        foreach (var grade in grades)
        {
            using var gradeId = ImRaii.PushId(grade);
            var ids = visible.SelectMany(t => t.Grades.Where(g => g.Grade == grade).Select(g => g.ItemId)).ToList();
            if (ids.Count == 0)
                continue;

            var allOn = ids.All(selected.Contains);
            if (ImGui.Button($"{(allOn ? "Clear" : "Select")} {IntExtensions.ToRomanNumeral(grade + 1)}"))
                SetMany(ids, !allOn);
            ImGui.SameLine();
        }
        ImGui.NewLine();

        ImGui.Separator();
        using var child = ImRaii.Child("##MateriaBlacklistList", Vector2.Zero, true);
        if (!child) return;

        foreach (var category in new[] { MateriaCategory.Battle, MateriaCategory.DoH, MateriaCategory.DoL })
        {
            var group = visible.Where(t => t.Category == category).ToList();
            if (group.Count == 0)
                continue;

            if (!ImGui.CollapsingHeader($"{CategoryName(category)} ({group.Count})", ImGuiTreeNodeFlags.DefaultOpen))
                continue;

            using var catIndent = ImRaii.PushIndent();
            foreach (var type in group)
            {
                using var typeId = ImRaii.PushId((int)type.MateriaRowId);
                var typeIds = type.Grades.Select(g => g.ItemId).ToList();
                var typeAllOn = typeIds.All(selected.Contains);
                var typeAnyOn = typeIds.Any(selected.Contains);

                if (ImGui.Button(typeAllOn ? "None" : "All", new Vector2(48, 0)))
                    SetMany(typeIds, !typeAllOn);
                ImGui.SameLine();
                ImGui.Text(type.Name);

                using var gradeIndent = ImRaii.PushIndent();
                var dirty = false;
                foreach (var grade in type.Grades)
                {
                    using var gId = ImRaii.PushId(grade.Grade);
                    var on = selected.Contains(grade.ItemId);
                    if (ImGui.Checkbox($"{IntExtensions.ToRomanNumeral(grade.Grade + 1)}##{grade.ItemId}", ref on))
                    {
                        if (on && !selected.Contains(grade.ItemId))
                            selected.Add(grade.ItemId);
                        else if (!on)
                            selected.Remove(grade.ItemId);
                        dirty = true;
                    }
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip($"{grade.ItemName} [{grade.ItemId}]");
                    ImGui.SameLine();
                }
                ImGui.NewLine();
                if (dirty)
                    C.Save();
            }
        }
    }

    private static void SetMany(IEnumerable<uint> itemIds, bool add)
    {
        var selected = C.TradeMultipleBlacklistItemIds;
        var changed = false;
        foreach (var id in itemIds)
        {
            if (add)
            {
                if (selected.Contains(id))
                    continue;
                selected.Add(id);
                changed = true;
            }
            else if (selected.Remove(id))
            {
                changed = true;
            }
        }

        if (changed)
            C.Save();
    }

    private static string CategoryName(MateriaCategory category) => category switch
    {
        MateriaCategory.DoH => "DoH",
        MateriaCategory.DoL => "DoL",
        _ => "Battle",
    };
}
