using System;

namespace Tgs.GameProviders.Pluggable.ViewModels;

public record GameRoundViewModel
{
	public Guid RoundId { get; init; }
	public string ExternalRoundId { get; init; } = default!;
	public Guid GameProviderId { get; init; }
	public string PlayerCode { get; init; } = default!;
	public int GameId { get; init; }
	public string CurrencyCode { get; init; } = default!;
	public int Status { get; init; }
	public Guid GameProviderConfigurationId { get; init; }
	public Guid WalletId { get; init; }
	public DateTimeOffset CreatedOn { get; init; }
	public DateTimeOffset? ClosedOn { get; init; }
	public DateTimeOffset UpdatedOn { get; init; }
	public Guid BrandId { get; init; }
	public DateTimeOffset? ExternalCreatedOn { get; init; }
	public DateTimeOffset? ExternalClosedOn { get; init; }
	public decimal? BetAmount { get; init; }
	public decimal? ValidBetAmount { get; init; }
	public decimal? SettleAmount { get; init; }
	public decimal? Commission { get; init; }
	public decimal? Turnover { get; init; }
	public decimal? WinLoss { get; init; }
	public decimal? TieBetAdjustment { get; init; }
	public int ClientStatus { get; init; }
}
