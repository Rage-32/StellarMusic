using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using StellarMusic.Extensions;

namespace StellarMusic.Commands;

public class MusicCommands : ApplicationCommandModule
{
    [SlashCommand("disconnect", "Leave the current voice channel.")]
    public async Task LeaveCommand(InteractionContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.RespondAsync("The Lavalink connection is not established");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
        {
            await ctx.RespondAsync("Not a valid voice channel.");
            return;
        }
        
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

        if (conn == null!)
        {
            await ctx.RespondAsync("Lavalink is not connected.");
            return;
        }

        await conn.DisconnectAsync();
        await ctx.RespondAsync($"Left `{ctx.Member.VoiceState.Channel.Name}`!");
    }
    
    [SlashCommand("play", "Play a song by name or URL.")]
    public async Task PlayCommand(InteractionContext ctx, [Option("song", "songURL")] string songUrl)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null!)
        {
            if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("You need to join a voice channel!", true);
                return;
            }

            await node.ConnectAsync(ctx.Member.VoiceState.Channel);
            conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
        }

        var loadResult = await node.Rest.GetTracksAsync(songUrl);
        
        if (loadResult.LoadResultType is LavalinkLoadResultType.LoadFailed or LavalinkLoadResultType.NoMatches)
        {
            await ctx.RespondAsync($"Track search failed for `{songUrl}`!", true);
            return;
        }

        var track = loadResult.Tracks.First();

        await conn.PlayAsync(track);

        var formattedTime = track.Length.Hours > 1 ? $"{track.Length.Hours}:{track.Length:mm\\:ss}" : $"{track.Length:mm\\:ss}";

        var embed = new DiscordEmbedBuilder
        {
            Title = "Now Playing",
            Description = $"[{track.Title}]({track.Uri})",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = $"https://img.youtube.com/vi/{track.Uri.ToString().Split("=")[1]}/0.jpg"
            },
            Color = DiscordColor.Blurple,
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"Requested by {ctx.Member.DisplayName}",
                IconUrl = ctx.Member.AvatarUrl
            }
        }.AddField("Duration", formattedTime);

        await ctx.RespondAsync(embed);
    }
    [SlashCommand("pause", "Pause the current song on the player.")]
    public async Task PauseCommand(InteractionContext ctx)
    {
        if (ctx.Member.VoiceState == null! || ctx.Member.VoiceState.Channel == null!)
        {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null!)
        {
            await ctx.RespondAsync("Lavalink is not connected.");
            return;
        }

        if (conn.CurrentState.CurrentTrack == null!)
        {
            await ctx.RespondAsync("There are no tracks loaded.");
            return;
        }

        await conn.PauseAsync();
        await ctx.RespondAsync("Paused the player.");
    }
    
    [SlashCommand("resume", "Resume the current song on the player.")]
    public async Task ResumeCommand(InteractionContext ctx)
    {
        if (ctx.Member.VoiceState == null! || ctx.Member.VoiceState.Channel == null!)
        {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null!)
        {
            await ctx.RespondAsync("Lavalink is not connected.");
            return;
        }

        if (conn.CurrentState.CurrentTrack == null!)
        {
            await ctx.RespondAsync("There are no tracks loaded.");
            return;
        }

        await conn.ResumeAsync();
        await ctx.RespondAsync("Resumed the player.");
    }
}