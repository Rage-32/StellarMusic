using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace StellarMusic.Events;

public static class GuildCreated
{
    public static async Task DiscordOnGuildCreated(DiscordClient sender, GuildCreateEventArgs args)
    {
        var logsChannelId = Config.Config.Current.ServerLogsChannelId;
        if (logsChannelId == 0) return;
        if (!sender.Guilds.Values.Any(server => server.Channels.ContainsKey(logsChannelId))) return;
        
        var channel = await sender.GetChannelAsync(logsChannelId);
        
        var embed = new DiscordEmbedBuilder
        {
            Description = "Joined a server!",
            Color = DiscordColor.SpringGreen,
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
        
        embed.AddField("Owner", $"`{args.Guild.Owner.Username}`");
        embed.AddField("Member Count", $"`{args.Guild.Members.Count}`");
        embed.AddField("Created", $"<t:{args.Guild.CreationTimestamp.ToUnixTimeSeconds()}:R>");
        
        await channel.SendMessageAsync(embed);
    }
}