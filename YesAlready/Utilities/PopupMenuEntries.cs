using Dalamud.Memory;

namespace YesAlready.Utils;

internal static unsafe class PopupMenuEntries
{
    public static string[] GetTexts(PopupMenu* popupMenu)
    {
        if (popupMenu == null || popupMenu->EntryCount <= 0 || popupMenu->EntryNames == null)
            return [];

        var entries = new string[popupMenu->EntryCount];
        for (var i = 0; i < popupMenu->EntryCount; i++)
            entries[i] = MemoryHelper.ReadSeStringNullTerminated((nint)popupMenu->EntryNames[i].Value).GetText();
        return entries;
    }

    public static (int Index, string Text)[] GetIndexed(PopupMenu* popupMenu)
    {
        var texts = GetTexts(popupMenu);
        var result = new (int Index, string Text)[texts.Length];
        for (var i = 0; i < texts.Length; i++)
            result[i] = (i, texts[i]);
        return result;
    }
}
