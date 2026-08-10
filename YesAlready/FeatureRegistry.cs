using clib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YesAlready;

public sealed class FeatureRegistry : IPluginService, IDisposable
{
    private readonly List<BaseFeature> _features = [];
    private readonly Dictionary<Type, BaseFeature> _byType = [];
    private readonly Dictionary<string, BaseFeature> _byName = [with(StringComparer.OrdinalIgnoreCase)];

    public IReadOnlyList<BaseFeature> All => _features;

    public FeatureRegistry()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!typeof(BaseFeature).IsAssignableFrom(type) || type.IsAbstract)
                continue;

            if (Activator.CreateInstance(type) is not BaseFeature feature)
                continue;

            _features.Add(feature);
            _byType[type] = feature;
            _byName[feature.Key] = feature;
        }
    }

    public void EnableAll()
    {
        foreach (var feature in _features)
            feature.Enable();
    }

    public void DisableAll()
    {
        foreach (var feature in _features)
            feature.Disable();
    }

    public T? GetFeature<T>() where T : BaseFeature
        => _byType.TryGetValue(typeof(T), out var feature) ? (T)feature : null;

    public BaseFeature? GetFeature(string name)
        => _byName.TryGetValue(name, out var feature) ? feature : null;

    public IEnumerable<(AddonFeature Feature, BotherAttribute Bother)> GetBothers()
    {
        foreach (var feature in _features.OfType<AddonFeature>())
        {
            var bothers = feature.Bothers.Length > 0 ? feature.Bothers : [.. feature.GetType().GetCustomAttributes<BotherAttribute>(true)];
            foreach (var bother in bothers)
                yield return (feature, bother);
        }
    }

    public void Dispose() => DisableAll();
}
