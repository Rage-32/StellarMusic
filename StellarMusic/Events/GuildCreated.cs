using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace StellarMusic.Events;

public static class GuildCreated
{
    public static async Task DiscordOnGuildCreated(DiscordClient sender, GuildCreateEventArgs args)
    {
        var channel = await sender.GetChannelAsync(1213702969049223188);
        
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