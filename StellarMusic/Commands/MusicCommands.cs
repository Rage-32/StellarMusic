using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using StellarMusic.Extensions;
using StellarMusic.Models;

namespace StellarMusic.Commands;

public class MusicCommands : ApplicationCommandModule
{
    public static Dictionary<DiscordGuild, List<Track>> ServerQueue = new(); // move this somewhere else

    [SlashCommandGroup("bassboost", "Bassboost commands")]
    public class BassBoostSubCommands : ApplicationCommandModule
    {
        [SlashCommand("set", "Set bass boost")]
        public async Task BassBoostCommand(InteractionContext ctx, [Option("bassboost", "Set the level of bass boost")] string level)
        {
            await ctx.CheckLinkValid(ctx.Client);
        
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);
    
            if (!float.TryParse(level, out var levels))
            {
                await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 Invalid bass boost level. Level must be between 0 and 0.9. (Use decimals)").WithColor(new DiscordColor(0xdd2e44)));
                return;
            }
            
            if (levels > 0.9 || levels < 0)
            {
                await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 Bass boost level must be between 0 and 0.9. (Use decimals)").WithColor(new DiscordColor(0xdd2e44)));
                return;
            }
    
            await conn.AdjustEqualizerAsync(new LavalinkBandAdjustment(1, levels));
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription($"🔊 Bass boost is now set to `{levels}f`!").WithColor(new DiscordColor(0xa388cd)));
        }
        
        [SlashCommand("reset", "Reset the bass boost")]
        public async Task ResetBassBoostCommand(InteractionContext ctx)
        {
            await ctx.CheckLinkValid(ctx.Client);
        
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);
    
            await conn.ResetEqualizerAsync();
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🟣 Successfully reset the equalizer!").WithColor(new DiscordColor(0xa388cd)));
        }
    }

    [SlashCommandGroup("queue", "Queue commands")]
    public class QueueSubCommands : ApplicationCommandModule
    {
        [SlashCommand("get", "Get the current queue")]
        public async Task QueueGetCommand(InteractionContext ctx)
        {
            try
            {
                await ctx.CheckLinkValid(ctx.Client);
        
                if (!ServerQueue.ContainsKey(ctx.Guild) || ServerQueue[ctx.Guild].Count < 1)
                {
                    await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 There are no tracks in the queue.").WithColor(new DiscordColor(0xdd2e44)));
                    return;
                }

                await ctx.DeferAsync();
            
                var queue = ServerQueue[ctx.Guild];

                var pageSize = Config.Config.Current.GetQueuePageLimit;

                var pages = new List<Page>();
                var pageCount = (int)Math.Ceiling(queue.Count / (double)pageSize);
            
                for (var i = 0; i < pageCount; i++)
                {
                    var tracks = queue.Skip(i * pageSize).Take(pageSize).ToList();
                    var page = new Page
                    {
                        Embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(0xa388cd),
                            Description = string.Join("\n", tracks.Select((track, index) => $"▶ #{i * pageSize + index + 1}: [{track.GetTrack.Title}]({track.GetTrack.Uri}) [{ctx.Guild.GetMemberAsync(track.RequestedBy).Result.Username}]"))
                        }
                    };
                    pages.Add(page);
                }
            
                await ctx.Interaction.SendPaginatedResponseAsync(false, ctx.User, pages, asEditResponse: true, deletion: ButtonPaginationBehavior.Disable);
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 Failed to load the current queue.").WithColor(new DiscordColor(0xdd2e44)));
                ctx.Client.Logger.LogError(e, "Failed to load the current queue");
            }
        }
        
        [SlashCommand("clear", "Clear the current queue.")]
        public async Task QueueClearCommand(InteractionContext ctx)
        {
            await ctx.CheckLinkValid(ctx.Client);
            
            if (!ServerQueue.ContainsKey(ctx.Guild))
            {
                await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 There are no tracks in the queue.").WithColor(new DiscordColor(0xdd2e44)));
                return;
            }
            
            ServerQueue[ctx.Guild] =
            [
                new Track
                {
                    Repeat = ServerQueue[ctx.Guild].FirstOrDefault().Repeat,
                    RequestedBy = ServerQueue[ctx.Guild].FirstOrDefault().RequestedBy,
                    GetTrack = ServerQueue[ctx.Guild].FirstOrDefault().GetTrack
                }
            ];
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🟣 Cleared the queue.").WithColor(new DiscordColor(0xa388cd)));
        }
    }

    [SlashCommand("repeat", "Repeat the current song")]
    public async Task RepeatCommand(InteractionContext ctx)
    {
        await ctx.CheckLinkValid(ctx.Client);

        if (ServerQueue.ContainsKey(ctx.Guild) && ServerQueue[ctx.Guild].Count > 0)
        {
            ServerQueue[ctx.Guild].First().Repeat = !ServerQueue[ctx.Guild].First().Repeat;
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription($"🔁 Repeat is now `{(ServerQueue[ctx.Guild].First().Repeat ? "enabled" : "disabled")}`!").WithColor(new DiscordColor(0x7289da)));
        }
        else
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 There are no tracks in the queue.").WithColor(new DiscordColor(0xdd2e44)));
        }
    }
    
    [SlashCommand("stop", "Stop the player, clear the queue, and disconnect from the voice channel.")]
    public async Task StopCommand(InteractionContext ctx)
    {
        await ctx.CheckLinkValid(ctx.Client);
        
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

        await conn.StopAsync();
        await conn.DisconnectAsync(false);
        ServerQueue.Remove(ctx.Guild);
        
        await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🟣 Stopped the player.").WithColor(new DiscordColor(0xa388cd)));
    }

    [SlashCommand("skip", "Skip the current song and go to the next")]
    public async Task SkipCommand(InteractionContext ctx)
    {
        await ctx.CheckLinkValid(ctx.Client);
        await ctx.DeferAsync();
        
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);
        
        if (ServerQueue.ContainsKey(ctx.Guild) && ServerQueue[ctx.Guild].Count > 1)
        {
            await conn.StopAsync();
            await Task.Delay(1000);
            var track = conn.CurrentState.CurrentTrack;
            await ctx.EditAsync(new DiscordEmbedBuilder()
                .WithDescription($"🎵 Now playing [{track.Title}]({track.Uri})")
                .WithColor(new DiscordColor(0x7289da))
                .WithThumbnail(conn.CurrentState.CurrentTrack.Uri.GetTrackThumbnail()));
        }
        else
        {
            await ctx.EditAsync(new DiscordEmbedBuilder()
                .WithDescription("🔴 There are no more tracks in the queue. Use `/stop` to stop the player.")
                .WithColor(new DiscordColor(0xdd2e44)));
        }
    }

    [SlashCommand("current", "Get the current song")]
    public async Task CurrentCommand(InteractionContext ctx)
    {
        try
        {
            await ctx.CheckLinkValid(ctx.Client);
        
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

            var track = conn.CurrentState.CurrentTrack;
            await ctx.RespondAsync(new DiscordEmbedBuilder()
                .WithDescription($"▶ Currently playing: [{track.Title}]({track.Uri})")
                .WithColor(new DiscordColor(0xa388cd))
                .WithFooter($"{track.Author}")
                .WithTimestamp(DateTime.Now));
        }
        catch
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🔴 Failed to get current track.").WithColor(new DiscordColor(0xdd2e44)), true);
        }
    }
    
    [SlashCommand("volume", "Set the volumne")]
    public async Task VolumeCommand(InteractionContext ctx, [Option("volume", "The volume level to set")] string volumeString)
    {
        await ctx.CheckLinkValid(ctx.Client);
        
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

        var parse = Int32.TryParse(volumeString, out var volume);
        if (!parse)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription($"🔴 `{volumeString}` is not a valid volume level.").WithColor(new DiscordColor(0xdd2e44)), true);
            return;
        }
        
        if (volume is > 100 or < 0)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription($"🔴 Volume cannot be higher than 100 or lower than 0.").WithColor(new DiscordColor(0xdd2e44)), true);
            return;
        }

        await conn.SetVolumeAsync(volume);
        await ctx.RespondAsync(new DiscordEmbedBuilder()
            .WithDescription($"{(volume >= 75 ? "🔊" : volume >= 9 ? "🔉" : volume >= 1 ? "🔈" : volume == 0 ? "🔇" : "🔇")} {(volume == 0 ? "Successfully muted the player" : $"Set the players volume to `{volume}%`")}.")
            .WithColor(new DiscordColor(0xa388cd)));
    }
    
    [SlashCommand("pause", "Pause the current song on the player.")]
    public async Task PauseCommand(InteractionContext ctx)
    {
        await ctx.CheckLinkValid(ctx.Client);
        
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

        await conn.PauseAsync();
        await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🟣 Paused the player.").WithColor(new DiscordColor(0xa388cd)));
    }
    
    [SlashCommand("resume", "Resume the current song on the player.")]
    public async Task ResumeCommand(InteractionContext ctx)
    {
        await ctx.CheckLinkValid(ctx.Client);
        
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

        await conn.ResumeAsync();
        await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription("🟣 Resumed the player.").WithColor(new DiscordColor(0xa388cd)));
    }
    
    [SlashCommand("play", "Play a song by name or URL.")]
    public async Task PlayCommand(InteractionContext ctx, [Option("song", "songURL")] string songUrl)
    {
        await ctx.DeferAsync();
        
        try
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            
            if (ctx.Member.VoiceState is null || ctx.Member.VoiceState.Channel is null || ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.EditAsync(new DiscordEmbedBuilder().WithDescription("🔴 You are not in a valid voice channel.").WithColor(new DiscordColor(0xdd2e44)));
                return;
            }
            
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
    
            if (conn == null!)
            {
                await node.ConnectAsync(ctx.Member.VoiceState.Channel);
                conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            }
    
            var loadResult = await node.Rest.GetTracksAsync(songUrl);
            
            if (loadResult.LoadResultType is LavalinkLoadResultType.LoadFailed or LavalinkLoadResultType.NoMatches)
            {
                await ctx.EditAsync(new DiscordEmbedBuilder().WithDescription($"🔴 Track search failed for `{songUrl}`! Please try again.").WithColor(new DiscordColor(0xdd2e44)));
                return;
            }
            
            var track = loadResult.Tracks.First();
            
            if (track.IsStream)
            {
                await ctx.EditAsync(new DiscordEmbedBuilder().WithDescription("🔴 Cannot play live streams.").WithColor(new DiscordColor(0xdd2e44)));
                return;
            }
            
            if (ServerQueue.ContainsKey(ctx.Guild) && ServerQueue[ctx.Guild].Count > 0)
            {
                ServerQueue[ctx.Guild].Add(new Track
                {
                    Repeat = false,
                    GetTrack = track,
                    RequestedBy = ctx.User.Id
                });
                
                var pos = ServerQueue[ctx.Guild].IndexOf(ServerQueue[ctx.Guild].Last());
                
                await ctx.EditAsync(new DiscordEmbedBuilder().WithDescription($"🎵 Added [{track.Title}]({track.Uri}) to the queue! Position: #{pos + 1}").WithColor(new DiscordColor(0x7289da)));
                return;
            }
    
            await conn.PlayAsync(track);
            ServerQueue.Add(ctx.Guild, [
                new Track
                {
                    Repeat = false,
                    GetTrack = track,
                    RequestedBy = ctx.User.Id
                }
            ]);
    
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
    
            await ctx.EditAsync(embed);
        }
        catch (Exception e)
        {
            await ctx.EditAsync(new DiscordEmbedBuilder().WithDescription("🔴 Failed playing song.").WithColor(new DiscordColor(0xdd2e44)));
            ctx.Client.Logger.LogError(e, "Failed playing song");
        }
    }
}