using clib.Services;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YesAlready.Utils;

public sealed class MateriaCatalog : IPluginService
{
    private static readonly HashSet<uint> DohBaseParams = [11, 70, 71]; // CP, Craftsmanship, Control
    private static readonly HashSet<uint> DolBaseParams = [10, 72, 73]; // GP, Gathering, Perception

    private readonly Dictionary<uint, MateriaItemInfo> _byItemId;

    public int CurrentGradeCount { get; }
    public IReadOnlyList<MateriaTypeInfo> CurrentTypes { get; } // any whose grade count equals the sheet wide max
    public IReadOnlyList<byte> CurrentGrades { get; }

    public MateriaCatalog()
    {
        var rows = new List<(uint MateriaRowId, string TypeName, MateriaCategory Category, List<(byte Grade, uint ItemId, string ItemName)> Grades)>();

        foreach (var row in Materia.Rows)
        {
            if (row.BaseParam.RowId == 0)
                continue;

            var grades = new List<(byte Grade, uint ItemId, string ItemName)>();
            for (byte grade = 0; grade < row.Item.Count; grade++)
            {
                var itemRef = row.Item[grade];
                if (itemRef.RowId == 0 || !itemRef.IsValid)
                    continue;

                var itemName = itemRef.Value.Name.ToString();
                if (string.IsNullOrEmpty(itemName))
                    continue;

                grades.Add((grade, itemRef.RowId, itemName));
            }

            if (grades.Count == 0)
                continue;

            rows.Add((row.RowId, TypeNameFromItems(grades), CategoryFromBaseParam(row.BaseParam.RowId), grades));
        }

        CurrentGradeCount = rows.Count == 0 ? 0 : rows.Max(r => r.Grades.Count);
        _byItemId = [];

        foreach (var row in rows)
        {
            var isCurrent = row.Grades.Count == CurrentGradeCount; // if it's not current, it will be automatically blacklisted
            foreach (var (grade, itemId, itemName) in row.Grades)
            {
                _byItemId.TryAdd(itemId, new MateriaItemInfo(itemId, itemName, row.MateriaRowId, row.TypeName, grade, row.Category, isCurrent));
            }
        }

        CurrentTypes = [.. _byItemId.Values
            .Where(i => i.IsCurrent)
            .GroupBy(i => i.MateriaRowId)
            .Select(g =>
            {
                var first = g.First();
                var grades = g.OrderBy(x => x.Grade)
                    .Select(x => new MateriaGradeInfo(x.Grade, x.ItemId, x.ItemName))
                    .ToList();
                return new MateriaTypeInfo(first.MateriaRowId, first.TypeName, first.Category, grades);
            })
            .OrderBy(t => t.Category)
            .ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase)];

        CurrentGrades = [.. _byItemId.Values
            .Where(i => i.IsCurrent)
            .Select(i => i.Grade)
            .Distinct()
            .OrderBy(g => g)];
    }

    public bool TryGet(uint itemId, out MateriaItemInfo info) => _byItemId.TryGetValue(itemId, out info);
    public bool IsExcludedFromTransmute(uint itemId) => C.TradeMultipleBlacklistItemIds.Contains(itemId) || !_byItemId.TryGetValue(itemId, out var info) || !info.IsCurrent;

    private static string TypeNameFromItems(List<(byte Grade, uint ItemId, string ItemName)> grades)
    {
        var name = grades[0].ItemName;
        var idx = name.LastIndexOf(" Materia", StringComparison.OrdinalIgnoreCase);
        return idx > 0 ? name[..idx] : name;
    }

    private static MateriaCategory CategoryFromBaseParam(uint baseParamId)
    {
        if (DohBaseParams.Contains(baseParamId))
            return MateriaCategory.DoH;
        if (DolBaseParams.Contains(baseParamId))
            return MateriaCategory.DoL;
        return MateriaCategory.Battle;
    }
}

public enum MateriaCategory
{
    Battle,
    DoH,
    DoL,
}

public readonly record struct MateriaItemInfo(uint ItemId, string ItemName, uint MateriaRowId, string TypeName, byte Grade, MateriaCategory Category, bool IsCurrent);
public readonly record struct MateriaGradeInfo(byte Grade, uint ItemId, string ItemName);
public readonly record struct MateriaTypeInfo(uint MateriaRowId, string Name, MateriaCategory Category, List<MateriaGradeInfo> Grades);
