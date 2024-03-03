using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace StellarMusic.Events;

public static class GuildDeleted
{
    public static async Task DiscordOnGuildDeleted(DiscordClient sender, GuildDeleteEventArgs args)
    {
        var channel = await sender.GetChannelAsync(1213702969049223188);
        
        var embed = new DiscordEmbedBuilder
        {
            Description = "Left a server!",
            Color = DiscordColor.Red,
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = args.Guild.Name,
                IconUrl = args.Guild.IconUrl
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"Stellar Music | Server ID: {args.Guild.Id}",
                IconUrl = sender.CurrentUser.AvatarUrl
            }
        };

        await channel.SendMessageAsync(embed);
    }
}