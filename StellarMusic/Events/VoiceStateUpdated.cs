using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using StellarMusic.Commands;

namespace StellarMusic.Events;

public static class VoiceStateUpdated
{
    public static async Task DiscordOnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs args)
    {
        if (args.Before?.Channel != null && args.After?.Channel == null)
        {
            var lava = sender.GetLavalink();
            var node = lava.ConnectedNodes.Values.FirstOrDefault();
            var conn = node?.GetGuildConnection(args.Guild);
            if (conn is null) return;
            if (conn.Channel.Users.Count == 1 && conn.Channel.Users.FirstOrDefault().Id == sender.CurrentUser.Id)
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                if (conn.Channel.Users.Count == 1 && conn.Channel.Users.FirstOrDefault().Id == sender.CurrentUser.Id)
                {
                    if (!conn.IsConnected) return;
                    await conn.DisconnectAsync();
                    MusicCommands.ServerQueue.Remove(args.Guild);
                    var channel = CommandRan.GuildLastCommandRanChannel;
                    
                    if (CommandRan.GuildLastCommandRanChannel.ContainsKey(args.Guild.Id))
                    {
                        var lastChannel = args.Guild.GetChannel(channel[args.Guild.Id]);
                        if (lastChannel is null) return;
                        await lastChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithDescription("🕒 I have the left voice channel and reset the queue due to inactivity.").WithColor(new DiscordColor(0xa388cd))));
                    }
                }
            }
        }
        
        if (!args.User.IsBot) return;

        var oldChannel = args.Before?.Channel;
        var newChannel = args.After?.Channel;
        
        if (oldChannel == null && newChannel != null)
        {
            var lava = sender.GetLavalink();
            var node = lava.ConnectedNodes.Values.FirstOrDefault();

            await Task.Delay(3000); // Wait for player to start playing music
            var conn = node?.GetGuildConnection(args.Guild);

            var defaultVolume = Config.Config.Current.DefaultVolume;
            if (conn is null) return;
            await conn.SetVolumeAsync(defaultVolume);
            
            if (args.After.Member.VoiceState.IsSelfMuted) return;
            await args.After.Member.ModifyAsync(x => x.Deafened = true);
        }
        else if (oldChannel != null && newChannel == null)
        {
            MusicCommands.ServerQueue.Remove(args.Guild);
        }
    }
}