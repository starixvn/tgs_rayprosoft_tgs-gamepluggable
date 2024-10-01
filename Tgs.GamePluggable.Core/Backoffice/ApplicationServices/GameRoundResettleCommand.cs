using System;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record GameRoundResettleCommand(
	Guid RoundId,
	decimal BetAmount,
	decimal ValidBetAmount,
	decimal SettleAmount,
	decimal Commission,
	decimal Turnover,
	decimal CompanyWinLoss,
	Enums.RoundClientStatus ClientStatus,
	DateTimeOffset ExternalClosedOn,
	Guid OperatorId,
	string Remark,
	decimal? AdjustAmount);
