using System.ComponentModel.DataAnnotations;

namespace Tgs.GamePluggable.ViewModels.Operator;

public record GameProviderFundViewModel
{
	[Required]
	[Range(minimum: 1.0, maximum: 10000000.00)]
	public decimal Amount { get; init; }
}
