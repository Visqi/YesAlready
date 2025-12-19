namespace YesAlready.Features;

[AddonFeature(AddonEvent.PostSetup)]
internal class MiragePrismRemove : AddonFeature
{
    protected override bool IsEnabled() => C.MiragePrismRemoveDispel;

    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo, AtkUnitBase* atk) => new AddonMaster.MiragePrismRemove(atk).Dispel();

    public partial class AddonMaster
    {
        public class MiragePrismRemove : AddonMasterBase<AtkUnitBase>
        {
            public unsafe AtkComponentButton* DispelButton => Addon->GetComponentButtonById(14);
            public override string AddonDescription { get; } = "Remove glamour window";

            public MiragePrismRemove(nint addon) : base(addon) { }
            public unsafe MiragePrismRemove(void* addon) : base(addon) { }

            public unsafe void Dispel() => ClickButtonIfEnabled(DispelButton);
        }
    }
}
