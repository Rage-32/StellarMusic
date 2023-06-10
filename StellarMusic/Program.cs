using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using StellarMusic.Commands;

namespace StellarMusic;

public static class Program
{
    public static DiscordClient Discord;
    public static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }
    private static async Task MainAsync()
    {
        Discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = "MTExNTE3NTI5NTk5NDA0MDM1MA.GMTLzm.2ASPl7kogSYvvGEnCZ6hzqN-nzHt0rq8KcCabo",
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            LogUnknownEvents = false,
            MinimumLogLevel = LogLevel.Debug
        });

        var endpoint = new ConnectionEndpoint
        {
            Hostname = "127.0.0.1",
            Port = 3000
        };

        var lavalinkConfig = new LavalinkConfiguration
        {
            Password = "iloverage32",
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint
        };

        Discord.Ready += DiscordOnReady;
        var slash = Discord.UseSlashCommands();
        slash.RegisterCommands<MusicCommands>();
        var lavalink = Discord.UseLavalink();

        await Discord.ConnectAsync();
        await lavalink.ConnectAsync(lavalinkConfig);
        
        await Task.Delay(-1);
    }

    private static async Task DiscordOnReady(DiscordClient sender, ReadyEventArgs args)
    {
        Console.WriteLine("Started. Logged in as " + sender.CurrentUser.Username + "#" + sender.CurrentUser.Discriminator + " (" + sender.CurrentUser.Id + ")");
    }
}