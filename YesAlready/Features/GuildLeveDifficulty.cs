namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.GuildLeveDifficultyConfirm), BotherCategory.Other, "Automatically confirms guild leves upon initiation at the highest difficulty.")]
internal class GuildLeveDifficulty : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var atk = addonInfo.GetAddon<AtkUnitBase>();
        Callback.Fire(atk, true, 0, atk->AtkValues[1].Int);
    }
}
