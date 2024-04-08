using System.ComponentModel.DataAnnotations;

namespace Tgs.GamePluggable.ViewModels.Operator;

public record GameProviderFundViewModel
{
	[Required]
	[Range(minimum: (int)decimal.Zero, maximum: (double)decimal.MaxValue)]
	public decimal Amount { get; init; }
}
