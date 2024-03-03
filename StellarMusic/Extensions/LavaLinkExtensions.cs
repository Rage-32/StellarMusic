using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace StellarMusic.Extensions;

public static class LavaLinkExtensions
{
    public static string GetTrackThumbnail(this Uri uri)
    {
        return $"https://img.youtube.com/vi/{uri.ToString().Split("=")[1]}/0.jpg";
    }
    
    public static async Task CheckLinkValid(this InteractionContext ctx, DiscordClient client)
    {
        var lava = client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 The Lavalink connection is not established.").WithColor(new DiscordColor(0xdd2e44)), true);
            return;
        }

        var node = lava.ConnectedNodes.Values.First();
            
        if (ctx.Member.VoiceState is null || ctx.Member.VoiceState.Channel is null || ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 You are not in a valid voice channel.").WithColor(new DiscordColor(0xdd2e44)), true);
            return;
        }
        
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

        if (conn == null)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 Stellar Music is not connected.").WithColor(new DiscordColor(0xdd2e44)), true);
        }
    }
}