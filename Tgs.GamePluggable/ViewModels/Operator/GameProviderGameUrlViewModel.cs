using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GamePluggable.ViewModels.Operator;

public record GameProviderGameUrlViewModel
{
	[Required]
	[DefaultValue(typeof(string), "4ucmqplxb029")]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(int), "0")]
	public int GameId { get; init; }
	[Required]
	[DefaultValue(typeof(Guid), "00000000-0000-0000-0000-000000000000")]
	public Guid LoginRecordId { get; init; }
	[Required]
	[DefaultValue(typeof(string), "zh-CN")]
	public string Language { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(bool), "false")]
	public bool IsMobile { get; init; }
	[Required]
	[DefaultValue(typeof(string), "127.0.0.1")]
	public string IpAddress { get; init; } = default!;
	[DefaultValue(typeof(string), "https://dev-t1.rpgp.cc")]
	public Uri? BackUrl { get; init; }
}
