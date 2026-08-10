using System;

namespace YesAlready.BaseFeatures;

public enum BotherCategory
{
    Desynthesis,
    Melding,
    Retainers,
    Duties,
    PvP,
    Minigames,
    Shops,
    Glamour,
    Other,
    Forays,
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class BotherAttribute(string configProperty, BotherCategory category, string description, string? label = null) : Attribute
{
    public string ConfigProperty { get; } = configProperty;
    public BotherCategory Category { get; } = category;
    public string Description { get; } = description;
    public string? Label { get; } = label;
    public bool ContributesToEnable { get; init; } = true;
    public string? MutuallyExclusiveWith { get; init; }
    public string? RequiresEnabledProperty { get; init; }
}
