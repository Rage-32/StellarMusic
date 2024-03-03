using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using StellarMusic.Commands;
using StellarMusic.Events;
using StellarMusic.Events.LavaLink;

namespace StellarMusic;

public static class Program
{
    public static DiscordClient Discord;
    
    public static void Main()
    {
        Config.Config.Initialize();
        MainAsync().GetAwaiter().GetResult();
    }
    
    private static async Task MainAsync()
    {
        Discord = new DiscordClient(new DiscordConfiguration
        {
            Token = Config.Config.Current.Token ?? "",
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            LogUnknownEvents = false,
            MinimumLogLevel = LogLevel.Debug
        });

        var endpoint = new ConnectionEndpoint
        {
            Hostname = Config.Config.Current.EndpointHost ?? "",
            Port = Config.Config.Current.EndpointPort ?? 0
        };

        var lavaLinkConfig = new LavalinkConfiguration
        {
            Password = Config.Config.Current.LavaLinkPassword ?? "",
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint
        };

        Discord.SessionCreated += SessionCreated.DiscordOnSessionCreated;
        Discord.GuildCreated += GuildCreated.DiscordOnGuildCreated;
        Discord.GuildDeleted += GuildDeleted.DiscordOnGuildDeleted;
        
        var slash = Discord.UseSlashCommands();
        slash.RegisterCommands<MusicCommands>();

        slash.SlashCommandInvoked += CommandRan.SlashOnSlashCommandInvoked;
        slash.SlashCommandExecuted += CommandRan.SlashOnSlashCommandExecuted;
        
        var lavaLink = Discord.UseLavalink();

        await Discord.ConnectAsync();
        await lavaLink.ConnectAsync(lavaLinkConfig);
        
        lavaLink.ConnectedNodes[endpoint].PlaybackFinished += PlaybackFinished.OnPlaybackFinished;
        
        await Task.Delay(-1);
    }
}
