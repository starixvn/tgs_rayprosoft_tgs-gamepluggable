using System;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

public record GameRoundAddCommand(
    string ExternalRoundId,
    Guid GameProviderId,
    string PlayerCode,
    int GameId,
    string CurrencyCode,
    Guid WalletId,
    Guid BrandId,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal CompanyWinLoss,
    GameRoundClientStatus ClientRoundStatus,
    Guid OperatorId,
    string? Remark);
