using System.Diagnostics;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using StellarMusic.Extensions;

namespace StellarMusic.Events;

public static class CommandRan
{
    private static Stopwatch commandExecutionTimer;

    public static async Task SlashOnSlashCommandInvoked(SlashCommandsExtension sender, SlashCommandInvokedEventArgs args)
    {
        commandExecutionTimer = Stopwatch.StartNew();
    }
    
    public static async Task SlashOnSlashCommandExecuted(SlashCommandsExtension sender, SlashCommandExecutedEventArgs args)
    {
        commandExecutionTimer.Stop();
        
        var executionTime = commandExecutionTimer.Elapsed;
        var logsChannel = await sender.Client.GetChannelAsync(1213703029384151140);

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

        if (args.Context.User.Id is not 817443146320576603)
            await logsChannel.SendMessageAsync(embed);
    }
}