using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace StellarMusic.Events;

public static class GuildDeleted
{
    public static async Task DiscordOnGuildDeleted(DiscordClient sender, GuildDeleteEventArgs args)
    {
        var logsChannelId = Config.Config.Current.ServerLogsChannelId;
        if (logsChannelId == 0) return;
        if (!sender.Guilds.Values.Any(server => server.Channels.ContainsKey(logsChannelId))) return;
        
        var channel = await sender.GetChannelAsync(logsChannelId);
        
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