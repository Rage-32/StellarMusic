using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace StellarMusic.Extensions;

public class Error
{ 
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "";
}

public static class LavaLinkExtensions
{
    public static string GetTrackThumbnail(this Uri uri)
    {
        return $"https://img.youtube.com/vi/{uri.ToString().Split("=")[1]}/0.jpg";
    }
    
    public static Task<Error> CheckLinkValid(this InteractionContext ctx, DiscordClient client)
    {
        var lava = client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            return Task.FromResult(new Error
            {
                Success = false,
                Message = "You are not in a valid voice channel."
            });
        }

        var node = lava.ConnectedNodes.Values.First();
            
        if (ctx.Member.VoiceState?.Channel is null || ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
        {
            return Task.FromResult(new Error
            {
                Success = false,
                Message = "You are not connected to a voice channel."
            });
        }
        
        var conn = node.GetGuildConnection(ctx.Guild);

        return Task.FromResult(new Error
        {
            Success = conn != null,
            Message = conn == null ? "Stellar Music is not connected." : ""
        });
    }
}