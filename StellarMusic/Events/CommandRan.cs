using System.Diagnostics;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using StellarMusic.Extensions;

namespace StellarMusic.Events;

public static class CommandRan
{
    private static Stopwatch commandExecutionTimer;

    public static Task SlashOnSlashCommandInvoked(SlashCommandsExtension sender, SlashCommandInvokedEventArgs args)
    {
        var logsChannelId = Config.Config.Current.CommandsLogsChannelId;
        if (logsChannelId == 0) return Task.CompletedTask;
        if (!sender.Client.Guilds.Values.Any(server => server.Channels.ContainsKey(logsChannelId))) return Task.CompletedTask;
        
        commandExecutionTimer = Stopwatch.StartNew();
        return Task.CompletedTask;
    }

    public static Dictionary<ulong, ulong> GuildLastCommandRanChannel { get; set; } = new(); // move this somewhere else
    
    public static async Task SlashOnSlashCommandExecuted(SlashCommandsExtension sender, SlashCommandExecutedEventArgs args)
    {
        if (!args.Context.Channel.IsPrivate) GuildLastCommandRanChannel[args.Context.Guild.Id] = args.Context.Channel.Id;
        
        var logsChannelId = Config.Config.Current.CommandsLogsChannelId;
        if (logsChannelId == 0) return;
        if (!sender.Client.Guilds.Values.Any(server => server.Channels.ContainsKey(logsChannelId))) return;
        
        commandExecutionTimer.Stop();
        
        var executionTime = commandExecutionTimer.Elapsed;
        var logsChannel = await sender.Client.GetChannelAsync(logsChannelId);

        var embed = new DiscordEmbedBuilder
        {
            Description = $"A slash command has been executed by `{args.Context.User.Username}` - `{args.Context.User.Id}`",
            Timestamp = DateTimeOffset.Now,
            Color = DiscordColorX.RandomColor(),
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"Server ID: {args.Context.Guild.Id}"
            },
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = args.Context.Guild.Name,
                IconUrl = args.Context.Guild.IconUrl
            }
        };

        if (args.Context.Member.IsOwner)
            embed.AddField("Guild Owner", "`Yes`");

        embed.AddField("Channel", $"`#{args.Context.Channel.Name}`");
        embed.AddField("Execution Time", $"`{executionTime.TotalSeconds:F2}s`");
        embed.AddField("Command", $"`{args.Context.QualifiedName}`");
        
        await logsChannel.SendMessageAsync(embed);
    }
}