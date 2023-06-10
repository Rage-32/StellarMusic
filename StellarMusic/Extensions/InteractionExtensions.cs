using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace StellarMusic.Extensions;

public static class InteractionContextX
{
    public static async Task RespondAsync(this InteractionContext ctx, string message, bool asEphemeral = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral(asEphemeral));
    }

    public static async Task RespondAsync(this InteractionContext ctx, DiscordEmbed embed, bool asEphemeral = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral(asEphemeral));
    }

    public static async Task RespondAsync(this InteractionContext ctx, DiscordMessageBuilder embed,
        bool asEphemeral = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed.Embed).AddComponents(embed.Components)
                .AsEphemeral(asEphemeral));
    }

    public static async Task RespondAsync(this DiscordInteraction ctx, string message, bool asEphemeral = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral(asEphemeral));
    }

    public static async Task RespondAsync(this DiscordInteraction ctx, DiscordEmbed embed, bool asEphemeral = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral(asEphemeral));
    }
    
    public static async Task RespondAsync(this ContextMenuContext ctx, string message, bool asEphemeral = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral(asEphemeral));
    }
}