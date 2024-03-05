using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using StellarMusic.Extensions;

namespace StellarMusic.Attributes;

public class RequireConnection : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        var check = await ctx.CheckLinkValid(ctx.Client);
        if (check.Success is false)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder().WithDescription($"🔴 {check.Message}").WithColor(new DiscordColor(0xdd2e44)), true);
        }

        return check.Success;
    }
}