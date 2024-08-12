using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GamePluggable.ViewModels.Operator;

public record GameProviderFundViewModel
{
	[Required]
	[Range(minimum: 1.0, maximum: 10000000.00)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Amount { get; init; }
}
