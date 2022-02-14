using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameRoundInfo(
        Guid RoundId,
        string ExternalRoundId,
        Guid GameProviderId,
        string PlayerCode,
        int GameId,
        string CurrencyCode,
        RoundStatus Status,
        Guid GameProviderConfigurationId,
        Guid WalletId,
        DateTimeOffset CreatedOn,
        DateTimeOffset? ClosedOn,
        DateTimeOffset UpdatedOn,
        Guid BrandId,
        DateTimeOffset? ExternalCreatedOn,
        DateTimeOffset? ExternalClosedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? SettleAmount,
        decimal? Commission,
        decimal? Turnover,
        decimal? WinLoss,
        decimal? TieBetAdjustment,
        RoundClientStatus ClientStatus,
        decimal? ReopenTurnoverAdjustment,
        DateTimeOffset? ReopenedOn,
        decimal? EffectiveBettingPercentage
        );
}
