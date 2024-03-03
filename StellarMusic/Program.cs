using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
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
    public static DiscordClient? Discord;
    
    public static void Main()
    {
        Config.Config.Initialize();
        MainAsync().GetAwaiter().GetResult();
    }
    
    private static async Task MainAsync()
    {
        Discord = new DiscordClient(new DiscordConfiguration
        {
            Token = Config.Config.Current.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            LogUnknownEvents = false,
            MinimumLogLevel = LogLevel.Debug
        });

        Discord.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromMinutes(2)
        });

        var endpoint = new ConnectionEndpoint
        {
            Hostname = Config.Config.Current.EndpointHost,
            Port = Config.Config.Current.EndpointPort
        };

        var lavaLinkConfig = new LavalinkConfiguration
        {
            Password = Config.Config.Current.LavaLinkPassword,
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint
        };

        Discord.SessionCreated += SessionCreated.DiscordOnSessionCreated;
        Discord.GuildCreated += GuildCreated.DiscordOnGuildCreated;
        Discord.GuildDeleted += GuildDeleted.DiscordOnGuildDeleted;
        Discord.VoiceStateUpdated += VoiceStateUpdated.DiscordOnVoiceStateUpdated;
        
        var slash = Discord.UseSlashCommands();
        slash.RegisterCommands<MusicCommands>();
        slash.RegisterCommands<Commands.Commands>();

        slash.SlashCommandInvoked += CommandRan.SlashOnSlashCommandInvoked;
        slash.SlashCommandExecuted += CommandRan.SlashOnSlashCommandExecuted;
        
        var lavaLink = Discord.UseLavalink();

        await Discord.ConnectAsync();
        await lavaLink.ConnectAsync(lavaLinkConfig);
        
        lavaLink.ConnectedNodes[endpoint].PlaybackFinished += PlaybackFinished.OnPlaybackFinished;
        
        await Task.Delay(-1);
    }
}
