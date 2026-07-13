using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using System;
using System.Linq;
using System.Text;

namespace YesAlready.Utils;

public static class Extensions
{
    public static string WithoutWhitespace(this string str)
        => new([.. str.Where(c => !char.IsWhiteSpace(c))]);

    public static bool ContainsIgnoreWhitespace(this string text, string pattern, StringComparison comparison = StringComparison.Ordinal)
        => text.WithoutWhitespace().Contains(pattern.WithoutWhitespace(), comparison);

    public static bool EqualsIgnoreSpecial(this ReadOnlySeString ross, string str)
    {
        // french has nbsps we need to ignore
        return ross.ExtractText().WithoutWhitespace().Equals(str.WithoutWhitespace(), StringComparison.InvariantCultureIgnoreCase);
    }

    public static void PrintPluginMessage(this IChatGui chat, string msg)
        => chat.Print(new XivChatEntry
        {
            Type = C.MessageChannel,
            Message = new SeStringBuilder()
                .AddUiForeground($"[{Name}] ", 45)
                .AddText(msg)
                .Build()
        });

    public static void PrintPluginMessage(this IChatGui chat, SeString msg)
    {
        chat.Print(new XivChatEntry
        {
            Type = C.MessageChannel,
            Message = new SeStringBuilder()
            .AddUiForeground($"[{Name}] ", 45)
            .Append(msg)
            .Build()
        });
    }

    public static void PrintPluginMessage(this IChatGui chat, SeStringBuilder sb) => chat.PrintPluginMessage(sb.BuiltString);
    public static void PrintPluginMessage(this IChatGui chat, StringBuilder? sb)
    {
        if (sb != null)
            chat.PrintPluginMessage(sb.ToString());
    }
}
