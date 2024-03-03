namespace StellarMusic.Models;

public class Config
{
    public string Token { get; set; } = "";
    public string EndpointHost { get; set; } = "";
    public int EndpointPort { get; set; } = 3000;
    public string LavaLinkPassword { get; set; } = "";
    public int DefaultVolume { get; set; } = 100;
    public int GetQueuePageLimit { get; set; } = 10;
    public ulong CommandsLogsChannelId { get; set; } = 0;
    public ulong ServerLogsChannelId { get; set; } = 0;
}