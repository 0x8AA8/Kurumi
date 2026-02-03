using Discord;
using Discord.WebSocket;

namespace nhitomi;

public sealed class AppSettings
{
    public DiscordSettings Discord { get; set; } = new();
    public HttpSettings Http { get; set; } = new();
    public FeedSettings Feed { get; set; } = new();

    public sealed class DiscordSettings
    {
        public string? Token { get; set; }
        public string? BotInvite { get; set; }
        public ulong? TestGuildId { get; set; }

        public StatusSettings Status { get; set; } = new();
        public GuildSettings Guild { get; set; } = new();

        public DiscordSocketConfig GetSocketConfig() => new()
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.GuildMessageReactions |
                             GatewayIntents.DirectMessages |
                             GatewayIntents.DirectMessageReactions |
                             GatewayIntents.MessageContent,
            AlwaysDownloadUsers = false,
            MessageCacheSize = 100,
            LogLevel = LogSeverity.Info
        };

        public sealed class StatusSettings
        {
            public double UpdateInterval { get; set; } = 60;
            public string[] Games { get; set; } = [];
        }

        public sealed class GuildSettings
        {
            public ulong GuildId { get; set; }
            public string? GuildInvite { get; set; }
            public ulong ErrorChannelId { get; set; }
        }
    }

    public sealed class HttpSettings
    {
        public bool EnableProxy { get; set; }
    }

    public sealed class FeedSettings
    {
        public bool Enabled { get; set; }
    }
}
