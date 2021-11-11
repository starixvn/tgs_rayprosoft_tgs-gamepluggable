using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameModel
    {
        public int GameId { get; set; }
        public GameStatus Status { get; set; }
        public string GameExternalId { get; set; } = default!;
        public Guid GameProviderId { get; set; }
        public GameType Type { get; set; }
        public GamePlatformType PlatformType { get; set; }
        public bool SubGame { get; set; }
        public string GameCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? SubGameProviderCode { get; set; }
    }
}
