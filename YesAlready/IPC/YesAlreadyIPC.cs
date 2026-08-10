using ECommons.EzIpcManager;

namespace YesAlready.IPC;

public class YesAlreadyIPC
{
    public YesAlreadyIPC() => EzIPC.Init(this);

    [EzIPC] public bool IsPluginEnabled() => P.Active;

    [EzIPC] public void SetPluginEnabled(bool state) => C.Enabled = state;

    [EzIPC] public bool IsBotherEnabled(string name) => FeatureRegistry.Get().GetFeature(name) is { Enabled: true };

    [EzIPC]
    public void SetBotherEnabled(string name, bool state)
    {
        var feature = FeatureRegistry.Get().GetFeature(name);
        if (feature is null) return;

        if (state)
            feature.Enable();
        else
            feature.Disable();
    }

    [EzIPC]
    public void PausePlugin(int milliseconds)
    {
        C.Enabled = false;
        Service.TaskManager.EnqueueDelay(milliseconds);
        Service.TaskManager.Enqueue(() => C.Enabled = true);
    }

    [EzIPC]
    public bool PauseBother(string name, int milliseconds)
    {
        var feature = FeatureRegistry.Get().GetFeature(name);
        if (feature is null || !feature.Enabled)
            return false;
        feature.Disable();
        Service.TaskManager.EnqueueDelay(milliseconds);
        Service.TaskManager.Enqueue(feature.Enable);
        return true;
    }
}
