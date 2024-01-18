using System;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

public record GameRoundResettleCommand(
    Guid RoundId,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal CompanyWinLoss,
    GameRoundClientStatus RoundClientStatus,
    DateTimeOffset ExternalClosedOn,
    Guid OperatorId,
    string Remark,
    decimal? AdjustAmount);
