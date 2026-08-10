namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
[Bother(nameof(Configuration.BannerPreviewUpdate), BotherCategory.Other, "Automatically update portraits.")]
internal class BannerPreview : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
        => Callback.Fire(addonInfo.GetAddon<AtkUnitBase>(), true, 0);
}
