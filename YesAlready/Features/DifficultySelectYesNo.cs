using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
public class DifficultySelectYesNo : AddonFeature
{
    protected override bool IsEnabled() => C.DifficultySelectYesNoEnabled;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk)
    {
        Log($"Selecting difficulty: {C.DifficultySelectYesNo}");
        GameMain.ExecuteCommand(823, (int)C.DifficultySelectYesNo);
    }

    public enum Difficulty
    {
        Normal = 0,
        Easy = 1,
        VeryEasy = 2,
    }
}
