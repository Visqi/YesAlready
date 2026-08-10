namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup, "LobbyDKTCheck")]
[AddonFeature(AddonEvent.PostSetup, "LobbyDKTCheckExec")]
[Bother(nameof(Configuration.DataCentreTravelConfirmEnabled), BotherCategory.Other, "Automatically accept the Data Center travel confirmation.")]
internal class LobbyDKTCheck : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 0);
}
