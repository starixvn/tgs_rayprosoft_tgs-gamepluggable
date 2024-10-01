using System;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

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
	Enums.RoundClientStatus ClientStatus,
	Guid OperatorId,
	string? Remark);
