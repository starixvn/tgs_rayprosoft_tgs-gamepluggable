using System.ComponentModel;

namespace Tgs.GameProviders.Pluggable.ViewModels;

public record TicketDetailViewModel
{
	public required string PlayerCode { get; init; }

	public required long RefId { get; set; }

	public string Language { get; init; } = default!;

	public string BaseCurrency { get; set; }

	public string PlayerUsername { get; set; }
}
