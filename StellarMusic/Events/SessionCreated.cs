using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace StellarMusic.Events;

public static class SessionCreated
{
    public static async Task DiscordOnSessionCreated(DiscordClient sender, SessionReadyEventArgs args)
    {
        Console.WriteLine("Started. Logged in as " + sender.CurrentUser.Username + "#" + sender.CurrentUser.Discriminator + " (" + sender.CurrentUser.Id + ")");
        await sender.UpdateStatusAsync(new DiscordActivity("/help", ActivityType.Playing), UserStatus.DoNotDisturb);
    }
}