using System;
using Tgs.GameRounds.Common;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record GameRoundResult(
	Guid RoundId,
	string ExternalRoundId,
	Guid GameProviderId,
	string PlayerCode,
	int GameId,
	string CurrencyCode,
	GameRounds.Common.Enums.RoundStatus Status,
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
	GameRounds.Common.Enums.RoundClientStatus ClientStatus);
