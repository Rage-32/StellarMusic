using DSharpPlus.Entities;

namespace StellarMusic.Extensions;

public static class DiscordColorX
{
    private static readonly List<DiscordColor> RandomColors = new(new[]
    {
        DiscordColor.Aquamarine,
        DiscordColor.Azure,
        DiscordColor.SpringGreen,
        DiscordColor.Cyan,
        DiscordColor.Purple,
        DiscordColor.Red,
        DiscordColor.Orange,
        DiscordColor.HotPink,
        DiscordColor.Blurple,
        DiscordColor.Rose,
        DiscordColor.Lilac,
        DiscordColor.Sienna,
        DiscordColor.Teal,
        DiscordColor.Turquoise,
        DiscordColor.Wheat,
        DiscordColor.Yellow,
        DiscordColor.Gray
    });

    private static readonly Random Random = new();

    private static DiscordColor GetRandomColor()
    {
        var randomNumber = Random.Next(0, RandomColors.Count);
        return RandomColors[randomNumber];
    }

    public static DiscordColor RandomColor()
    {
        return GetRandomColor();
    }
}