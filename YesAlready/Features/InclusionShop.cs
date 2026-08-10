using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace YesAlready.Features;

[AddonFeature(AddonEvent.PreSetup)]
[AddonFeature(AddonEvent.PostRefresh)]
[Bother(nameof(Configuration.InclusionShopRememberEnabled), BotherCategory.Shops, "Remember the last panel visited on the scrip exchange window.")]
internal class InclusionShop : AddonFeature
{
    protected override unsafe void HandleAddonEvent(AddonEvent eventType, AddonArgs addonInfo)
    {
        var agent = AgentInclusionShop.Instance();
        if (agent == null || agent->Data == null || !agent->Data->IsShopReady)
            return;

        var data = agent->Data;
        switch (eventType)
        {
            case AddonEvent.PreSetup:
                Restore(agent, data, addonInfo);
                break;

            case AddonEvent.PostRefresh:
                Remember(data->InclusionShopId, data->SelectedCategoryIndex, data->SelectedSubCategoryTab);
                break;
        }
    }

    private unsafe void Restore(AgentInclusionShop* agent, AgentInclusionShop.AgentData* data, AddonArgs addonInfo)
    {
        if (!TryGetRemembered(data->InclusionShopId, out var category, out var subCategory))
            return;

        if (category >= data->CategoryCount)
        {
            Log($"Remembered category {category} out of range (count={data->CategoryCount}), skipping");
            return;
        }

        Log($"Restoring shop {data->InclusionShopId}: category={category}, subcategory={subCategory}");

        if (addonInfo is AddonSetupArgs { AtkValueCount: > 2 } setup) // setup just reads [2], calling SelectCategory isn't enough at this point
            setup.AtkValues.Cast<AtkValue>()[2].SetUInt(category);

        agent->SelectCategory(category);
        if (!data->SelectSubCategory(subCategory))
            Log($"Remembered subcategory {subCategory} out of range (visible={data->VisibleSubCategoryCount})");
    }

    private void Remember(uint shopId, byte category, byte subCategory)
    {
        var saves = C.InclusionShopSaves;
        var index = saves.FindIndex(m => m.ShopId == shopId);
        if (index >= 0 && saves[index].Category == category && saves[index].SubCategory == subCategory)
            return;

        Log($"Remembering shop {shopId}: category={category}, subcategory={subCategory}");
        var memory = new Configuration.InclusionShopSave
        {
            ShopId = shopId,
            Category = category,
            SubCategory = subCategory,
        };

        if (index >= 0)
            saves[index] = memory;
        else
            saves.Add(memory);

        C.Save();
    }

    private static bool TryGetRemembered(uint shopId, out byte category, out byte subCategory)
    {
        if (C.InclusionShopSaves.FindIndex(m => m.ShopId == shopId) is not (>= 0 and var index))
        {
            category = 0;
            subCategory = 0;
            return false;
        }

        var memory = C.InclusionShopSaves[index];
        category = memory.Category;
        subCategory = memory.SubCategory;
        return true;
    }
}
