using DSharpPlus.Lavalink;

namespace StellarMusic.Models;

public class Track
{
    public LavalinkTrack? GetTrack { get; set; }
    public ulong RequestedBy { get; set; }
    public bool Repeat { get; set; }
}