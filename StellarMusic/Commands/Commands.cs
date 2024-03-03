using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using StellarMusic.Extensions;

namespace StellarMusic.Commands;

public class Commands : ApplicationCommandModule
{
    [SlashCommand("ping", "Checks the latency for the bot.")]
    public async Task PingCommand(InteractionContext ctx)
    {
        await ctx.RespondAsync($"🏓 Pong! `{ctx.Client.Ping}ms`", true);
    }

    [SlashCommand("help", "Shows the help command.")]
    public async Task HelpCommand(InteractionContext ctx)
    {
        await ctx.DeferAsync();
        var embed = new DiscordEmbedBuilder().WithColor(0x916c94);
        
        var globalCommands = await ctx.Client.GetGlobalApplicationCommandsAsync();
        var test = "";
        foreach (var command in globalCommands)
        {
            if (command.Name == "help") continue;
            if (command.Name == "queue")
            {
                test = command.Options.Where(option => option.Type == ApplicationCommandOptionType.SubCommand).Aggregate(test, (current, option) => current + $"queue {option.Name}\n\u2003 ▶ {option.Description}\n");
                continue;
            }
            test += $"{command.Name}\n\u2003 ▶ {command.Description}\n";
        }
        
        embed.AddField($"Commands", $"{test}");

        await ctx.EditAsync(embed);
    }
}