using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameInfo(
            int GameId,
            GameStatus Status,
            string GameExternalId,
            string? Description,
            string? EndpointPath,
            string? RuleUrl,
            Guid GameProviderId,
            GameType GameType,
            GamePlatformType PlatformType,
            bool SubGame,
            bool AvailableToBrand,
            bool HasTurnoverOnTieBet,
            bool Html5,
            string[]? AvailableCurrencies,
            string GameCode,
            string Name,
            string? Localization,
            string? SubGameProviderCode);
}
