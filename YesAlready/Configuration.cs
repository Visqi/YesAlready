using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YesAlready;

public partial class Configuration() : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool Enabled { get; set; } = true;
    public XivChatType MessageChannel { get; set; } = Svc.PluginInterface.GeneralChatType;

    public VirtualKey ForcedYesKey { get; set; } = VirtualKey.NO_KEY;
    public VirtualKey ForcedTalkKey { get; set; } = VirtualKey.NO_KEY;
    public VirtualKey DisableKey { get; set; } = VirtualKey.NO_KEY;
    public bool SeparateForcedKeys { get; set; } = false;
    public TextFolderNode RootFolder { get; private set; } = new TextFolderNode { Name = "/" };
    public TextFolderNode OkRootFolder { get; private set; } = new TextFolderNode { Name = "/" };
    public TextFolderNode ListRootFolder { get; private set; } = new TextFolderNode { Name = "/" };
    public TextFolderNode TalkRootFolder { get; private set; } = new TextFolderNode { Name = "/" };
    public TextFolderNode NumericsRootFolder { get; private set; } = new TextFolderNode { Name = "/" };
    public TextFolderNode CustomRootFolder { get; private set; } = new TextFolderNode { Name = "/" };
    public bool DesynthDialogEnabled { get; set; } = false;
    public bool DesynthBulkDialogEnabled { get; set; } = false;
    public bool MaterializeDialogEnabled { get; set; } = false;
    public bool MaterialAttachDialogEnabled { get; set; } = false;
    public bool OnlyMeldWhenGuaranteed { get; set; } = true;
    public bool MateriaRetrieveDialogEnabled { get; set; } = false;
    public bool ItemInspectionResultEnabled { get; set; } = false;
    public int ItemInspectionResultRateLimiter { get; set; } = 0;
    public bool RetainerTaskAskEnabled { get; set; } = false;
    public bool RetainerTaskResultEnabled { get; set; } = false;
    public bool GrandCompanySupplyReward { get; set; } = false;
    public bool ShopCardDialog { get; set; } = false;
    public bool ShopExchangeItemDialogEnabled { get; set; } = false;
    public bool JournalResultCompleteEnabled { get; set; } = false;
    public bool ContentsFinderConfirmEnabled { get; set; } = false;
    public bool ContentsFinderOneTimeConfirmEnabled { get; set; } = false;
    public bool InclusionShopRememberEnabled { get; set; } = false;
    public uint InclusionShopRememberCategory { get; set; } = 0;
    public uint InclusionShopRememberSubcategory { get; set; } = 0;
    public bool GuildLeveDifficultyConfirm { get; set; } = false;
    public bool FallGuysRegisterConfirm { get; set; } = false;
    public bool FallGuysExitConfirm { get; set; } = false;
    public bool RetainerTransferListConfirm { get; set; } = false;
    public bool RetainerTransferProgressConfirm { get; set; } = false;
    public bool DesynthesisResults { get; set; } = false;
    public bool AetherialReductionResults { get; set; } = false;
    public bool FashionCheckQuit { get; set; } = false;
    public bool LordOfVerminionQuit { get; set; } = false;
    public bool ChocoboRacingQuit { get; set; } = false;
    public bool PartyFinderJoinConfirm { get; set; } = false;
    public bool GimmickYesNo { get; set; } = false;
    public bool AutoCollectable { get; set; } = false;
    public bool LotteryWeeklyInput { get; set; } = false;
    public bool TradeMultiple { get; set; } = false;
    public TradeMultipleMode TransmuteMode { get; set; } = TradeMultipleMode.AllSame;
    public bool KupoOfFortune { get; set; } = false;
    public bool CustomDeliveries { get; set; } = false;
    public bool MKSRecordQuit { get; set; } = false;
    public bool FrontlineRecordQuit { get; set; } = false;
    public bool DataCentreTravelConfirmEnabled { get; set; } = false;
    public bool AirShipExplorationResultFinalize { get; set; } = false;
    public bool AirShipExplorationResultRedeploy { get; set; } = false;
    public bool MiragePrismRemoveDispel { get; set; } = false;
    public bool MiragePrismExecuteCast { get; set; } = false;
    public bool BannerPreviewUpdate { get; set; } = false;
    public bool WKSRewardClose { get; set; } = false;
    public bool WKSAnnounceHide { get; set; } = false;
    public bool DifficultySelectYesNoEnabled { get; set; } = false;
    public Features.DifficultySelectYesNo.Difficulty DifficultySelectYesNo { get; set; } = Features.DifficultySelectYesNo.Difficulty.VeryEasy;

    public List<CustomBother> CustomCallbacks { get; set; } = [];

    public class CustomBother
    {
        public string Name { get; set; } = string.Empty;
        public string Addon { get; set; } = string.Empty;
        public bool UpdateState { get; set; } = true;
        public string CallbackParams { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }

    public enum TradeMultipleMode
    {
        AllSame = 0,
        AllDifferent = 1,
    }

    public static Configuration Load(DirectoryInfo configDirectory)
    {
        var pluginConfigPath = new FileInfo(Path.Combine(configDirectory.Parent!.FullName, "YesAlready.json"));

        if (!pluginConfigPath.Exists)
            return new Configuration();

        var data = File.ReadAllText(pluginConfigPath.FullName);
        var conf = JsonConvert.DeserializeObject<Configuration>(data);
        return conf ?? new Configuration();
    }

    public void Save() => Svc.PluginInterface.SavePluginConfig(this);

    public IEnumerable<ITextNode> GetAllNodes() => new ITextNode[]
        {
            RootFolder,
            OkRootFolder,
            ListRootFolder,
            TalkRootFolder,
            NumericsRootFolder,
            CustomRootFolder,
        }
        .Concat(GetAllNodes(RootFolder.Children))
        .Concat(GetAllNodes(OkRootFolder.Children))
        .Concat(GetAllNodes(ListRootFolder.Children))
        .Concat(GetAllNodes(TalkRootFolder.Children))
        .Concat(GetAllNodes(NumericsRootFolder.Children))
        .Concat(GetAllNodes(CustomRootFolder.Children));

    public IEnumerable<ITextNode> GetAllNodes(IEnumerable<ITextNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;
            if (node is TextFolderNode folder)
            {
                var children = GetAllNodes(folder.Children);
                foreach (var childNode in children)
                {
                    yield return childNode;
                }
            }
        }
    }

    public bool TryFindParent(ITextNode node, out TextFolderNode? parent)
    {
        foreach (var candidate in GetAllNodes())
        {
            if (candidate is TextFolderNode folder && folder.Children.Contains(node))
            {
                parent = folder;
                return true;
            }
        }

        parent = null;
        return false;
    }

    public static void CreateNode<T>(TextFolderNode folder, bool createFolder, string? zoneName = null, bool? isYes = null) where T : ITextNode, new()
    {
        var newNode = new T { Enabled = true };
        var chosenFolder = folder;

        switch (newNode)
        {
            case TextEntryNode textNode:
                textNode.Text = Service.Watcher.LastSeenDialogText;
                if (isYes.HasValue)
                    textNode.IsYes = isYes.Value;
                break;
            case OkEntryNode okNode:
                okNode.Text = Service.Watcher.LastSeenOkText;
                break;
            case ListEntryNode listNode:
                listNode.Text = Service.Watcher.LastSeenListSelection;
                break;
            case TalkEntryNode talkNode:
                talkNode.Text = Service.Watcher.LastSeenTalkTarget;
                break;
            case NumericsEntryNode numericsNode:
                numericsNode.Text = Service.Watcher.LastSeenNumericsText;
                break;
        }

        if (zoneName != null && newNode is IZoneRestrictedNode zoneNode)
        {
            zoneNode.ZoneRestricted = true;
            zoneNode.ZoneText = zoneName;
        }

        if (createFolder)
        {
            var folderName = zoneName ?? chosenFolder?.Name ?? string.Empty;
            chosenFolder = folder.Children.OfType<TextFolderNode>().FirstOrDefault(node => node.Name == folderName);
            if (chosenFolder == null)
            {
                chosenFolder = new TextFolderNode { Name = folderName };
                folder.Children.Add(chosenFolder);
            }
        }

        chosenFolder.Children.Add(newNode);
    }

    public void Migrate()
    {
        IMigration[] migrations = [new V2()];
        foreach (var migration in migrations)
        {
            if (C.Version < migration.Version)
            {
                PluginLog.Information($"Migrating from {C.Version} to {migration.Version}");
                var c = C;
                migration.Migrate(ref c);
                c.Version = migration.Version;
                C = c;
            }
        }
    }

    public interface IMigration
    {
        int Version { get; }
        void Migrate(ref Configuration config);
    }

    public class V2 : IMigration
    {
        public int Version => 2;
        public void Migrate(ref Configuration config)
        {
            foreach (var bother in config.CustomCallbacks)
            {
                var node = new CustomEntryNode
                {
                    Enabled = bother.Enabled,
                    Addon = bother.Addon,
                    Text = bother.Name,
                    UpdateState = bother.UpdateState,
                    CallbackParams = bother.CallbackParams
                };
                config.CustomRootFolder.Children.Add(node);
            }
            config.CustomCallbacks.Clear();
        }
    }
}
