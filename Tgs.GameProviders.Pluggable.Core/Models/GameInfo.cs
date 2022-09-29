using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core.Models
{
    public record GameInfo(
        int GameId,
        string GameExternalId,
        string? EndpointPath,
        string? RuleUrl,
        Guid GameProviderId,
        GameType GameType,
        GamePlatformType PlatformType,
        string GameCode,
        string? SubGameProviderCode);
}
