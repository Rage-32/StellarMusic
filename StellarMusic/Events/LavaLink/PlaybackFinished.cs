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
            
                await args.Player.PlayAsync(newTrack.GetTrack);
            }
            else if (queue.FirstOrDefault() is not null && queue.FirstOrDefault().Repeat)
            {
                await args.Player.PlayAsync(queue.FirstOrDefault().GetTrack);
            }
            else 
            {
                MusicCommands.ServerQueue.Remove(args.Player.Guild);
                await args.Player.DisconnectAsync(false);
            }
        }
    }
}