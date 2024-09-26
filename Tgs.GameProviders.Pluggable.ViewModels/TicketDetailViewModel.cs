using System;

namespace Tgs.GameProviders.Pluggable.ViewModels;

public record TicketDetailViewModel
{
	public required string GameProviderCode { get; set; } 

	public required string PlayerCode { get; set; } 

	public required string ExternalRoundId { get; set; }

	public int GameId { get; set; }

	public DateTime SettleDate { get; set; }

	public string? ResultId { get; set; }

	public string? Language { get; set; }

	public string? BaseCurrency { get; set; }

	public string? PlayerUsername { get; set; }
}
