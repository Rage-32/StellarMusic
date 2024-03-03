using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using StellarMusic.Commands;

namespace StellarMusic.Events.LavaLink;

public static class PlaybackFinished
{
    public static async Task OnPlaybackFinished(LavalinkGuildConnection sender, TrackFinishEventArgs args)
    {
        var guild = args.Player.Guild;
        
        if (MusicCommands.ServerQueue.ContainsKey(guild))
        {
            var queue = MusicCommands.ServerQueue[guild];
            
            if (queue.Count > 1 && !queue.FirstOrDefault().Repeat)
            {
                queue.Remove(queue.FirstOrDefault());

                var newTrack = queue.FirstOrDefault();

                if (!args.Player.IsConnected) return; 
                await args.Player.PlayAsync(newTrack.GetTrack);

                if (CommandRan.GuildLastCommandRanChannel.ContainsKey(guild.Id))
                {
                    var lastChannel = guild.GetChannel(CommandRan.GuildLastCommandRanChannel[guild.Id]);
                    if (lastChannel is null) return;
                    
                    await lastChannel.SendMessageAsync(new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithDescription($"🟣 Now playing: [{newTrack.GetTrack.Title}]({newTrack.GetTrack.Uri}) [{newTrack.GetTrack.Length}]")
                            .WithColor(new DiscordColor(0xa388cd))));
                }
            }
            else if (queue.FirstOrDefault() is not null && queue.FirstOrDefault().Repeat)
            {
                if (!args.Player.IsConnected) return; 
                await args.Player.PlayAsync(queue.FirstOrDefault().GetTrack);
            }
            else 
            {
                MusicCommands.ServerQueue.Remove(args.Player.Guild);
                if (!args.Player.IsConnected) return;
                await args.Player.DisconnectAsync(false);
                
                if (CommandRan.GuildLastCommandRanChannel.ContainsKey(guild.Id))
                {
                    var lastChannel = guild.GetChannel(CommandRan.GuildLastCommandRanChannel[guild.Id]);
                    if (lastChannel is null) return;
                    
                    await lastChannel.SendMessageAsync(new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithDescription("🚫 I have left the voice channel due as the queue is empty.")
                            .WithColor(DiscordColor.Red)));
                }
            }
        }
    }
}