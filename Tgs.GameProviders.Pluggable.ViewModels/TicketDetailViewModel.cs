using System.ComponentModel;

namespace Tgs.GameProviders.Pluggable.ViewModels;

public record TicketDetailViewModel
{
	public required string GameProviderCode { get; init; }

	public required string PlayerCode { get; init; }

	public required long ExternalId { get; set; }

	public string? Language { get; init; }

	public string? BaseCurrency { get; set; }

	public string? PlayerUsername { get; set; }
}
