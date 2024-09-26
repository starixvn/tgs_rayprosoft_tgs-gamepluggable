using System;
using System.ComponentModel;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record GetRoundTicketDetailQuery
{
	public required string GameProviderCode { get; set; }

	public required string PlayerCode { get; set; }

	public required int GameId { get; set; }

	public required string ExternalRoundId { get; set; }

	public DateTime SettleDate { get; set; }

	[DefaultValue(typeof(string), "")]
	public string ResultId { get; set; }

	[DefaultValue(typeof(string), "en-US")]
	public string? Language { get; set; }

	[DefaultValue(typeof(string), null)]
	public string? BaseCurrency { get; set; }

	[DefaultValue(typeof(string), null)]
	public string? PlayerUsername { get; set; }
}
