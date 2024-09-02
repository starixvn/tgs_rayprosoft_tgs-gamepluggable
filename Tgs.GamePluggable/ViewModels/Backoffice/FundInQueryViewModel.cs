using System.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace Tgs.GamePluggable.ViewModels.Backoffice;

public record FundInQueryViewModel
{
	[Required]
	[DefaultValue(typeof(string), "4ucmqplxb029")]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "KY")]
	public string GameProviderCode { get; init; } = default!;
}
