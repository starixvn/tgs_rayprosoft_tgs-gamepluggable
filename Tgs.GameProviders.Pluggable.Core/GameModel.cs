using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameModel(
         int GameId,
         GameStatus Status,
         string GameExternalId,
         Guid GameProviderId,
         GameType GameType,
         GamePlatformType PlatformType,
         bool SubGame,
         string GameCode,
         string Name,
         string? SubGameProviderCode
        );
}
