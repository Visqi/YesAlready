using System;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.LotteryWeeklyInput), BotherCategory.Minigames, "Automatically purchase a Jumbo Cactpot ticket with a random number.")]
internal class LotteryWeeklyInput : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, new Random().Next(0, 10000));
}
