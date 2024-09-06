using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GamePluggable.ViewModels.Operator;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Controllers.Operator;

[Route("op/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
	/// <summary>
	/// Games the provider fund in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("fundin/{playerCode}/{gameProviderCode}")]
	public ValueTask GameProviderFundInAsync(
		[FromServices] ICommandService<FundInCommand> service,
		string playerCode = "4ucmqplxb029",
		string gameProviderCode = "KY",
		[FromBody] GameProviderFundViewModel source = default!)
	{
		var query = new FundInCommand(
			PlayerCode: playerCode,
			GameProviderCode: gameProviderCode,
			Amount: source.Amount);

		return service
			.ExecuteAsync(
			query,
			cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Games the provider fund out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("fundout/{playerCode}/{gameProviderCode}")]
	public ValueTask GameProviderFundOutAsync(
		[FromServices] ICommandService<FundOutCommand> service,
		string playerCode = "4ucmqplxb029",
		string gameProviderCode = "KY",
		[FromBody] GameProviderFundViewModel source = default!)
	{
		var query = new FundOutCommand(
			PlayerCode: playerCode,
			GameProviderCode: gameProviderCode,
			Amount: source.Amount);

		return service
			.ExecuteAsync(
			query,
			cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Gets the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <returns></returns>
	[HttpGet("balance/{playerCode}/{gameProviderCode}")]
	public async ValueTask<decimal> GetBalanceAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		string playerCode = "4ucmqplxb029",
		string gameProviderCode = "KY")
	{
		var query = new PlayerBalanceQuery(
			PlayerCode: playerCode,
			GameProviderCode: gameProviderCode);

		var result = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.FreeBalance;
	}

	/// <summary>
	/// Gets the game URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("gameurl")]
	public ValueTask<Uri> GetGameUrlAsync(
		[FromServices] IGetService<GameUrlQuery, Uri> service,
		[FromBody] GameProviderGameUrlViewModel source)
	{
		var query = new GameUrlQuery(
			PlayerCode: source.PlayerCode,
			LoginRecordId: source.LoginRecordId,
			PlatformType: source.IsMobile ? GameProviders.Domains.PlatformType.Mobile : GameProviders.Domains.PlatformType.Desktop,
			GameId: source.GameId,
			Language: source.Language,
			IpAddress: source.IpAddress,
			BackUrl: source.BackUrl,
			ExtraInfo: source.ExtraInfo);

		return service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted);
	}
}
