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
        
        embed.AddField("Commands", $"{test}");

        await ctx.EditAsync(embed);
    }

    [SlashCommand("info", "Information about Stellar Music.")]
    public async Task InfoCommand(InteractionContext ctx)
    {
        await ctx.DeferAsync();
    
        var commands = await ctx.Client.GetGlobalApplicationCommandsAsync();
        var commandsCount = commands.Count;
        var serversCount = ctx.Client.Guilds.Count;
    
        var embed = new DiscordEmbedBuilder().WithColor(new DiscordColor(0x916c94));
        embed.AddField("Source", "[GitHub](https://github.com/Rage-32)", true);
        embed.AddField("Framework", ".NET 8.0", true);
        embed.AddField("Library", "[DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)", true);
        embed.AddField("Commands", $"{commandsCount}", true);
        embed.AddField("Servers", $"{serversCount}", true);
        embed.AddField("Users", $"{ctx.Client.Guilds.Sum(s => s.Value.Members.Count)}", true);
        
        var inviteButton = new DiscordLinkButtonComponent("https://discord.com/api/oauth2/authorize?client_id=1115175295994040350&permissions=15747072&scope=bot%20applications.commands", "Invite", false, new DiscordComponentEmoji("📜")); // TODO: CHANGE THIS
        var supportButton = new DiscordLinkButtonComponent("https://discord.com/invite/aZEGSPKQMk", "Support Server", false, new DiscordComponentEmoji("🌎"));
        var websiteButton = new DiscordLinkButtonComponent("https://stellarbot.dev", "Website", false, new DiscordComponentEmoji("🔗"));

        var messageBuilder = new DiscordMessageBuilder()
            .AddEmbed(embed)
            .AddComponents(inviteButton, supportButton, websiteButton);

        await ctx.EditAsync(messageBuilder);
    }
}