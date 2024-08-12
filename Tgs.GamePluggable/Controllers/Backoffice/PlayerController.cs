using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GamePluggable.ViewModels.Backoffice;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Controllers.Backoffice;

[Route("bo/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
	/// <summary>
	/// Funds the in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("fund/in", Name = "Backoffice_Player_FundIn")]
	public ValueTask FundInAsync(
		[FromServices] ICommandService<FundInCommand> service,
		 FundInQueryViewModel source)
		=> service.ExecuteAsync(
			command: new FundInCommand(
				PlayerCode: source.PlayerCode,
				GameProviderCode: source.GameProviderCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <returns></returns>
	[HttpGet("balance/{playerCode}/{gameProviderCode}", Name = "Backoffice_Player_Balance")]
	public async ValueTask<decimal> PlayerBalanceAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		string playerCode = "4ucmqplxb029",
		string gameProviderCode = "KY")
	{
		var result = await service.GetAsync(
			query: new PlayerBalanceQuery(
				PlayerCode: playerCode,
				GameProviderCode: gameProviderCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result.FreeBalance;
	}

	/// <summary>
	/// Players the balance information asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <returns></returns>
	[HttpGet("balance/info/{playerCode}/{gameProviderCode}", Name = "Backoffice_Player_Balance_Info")]
	public async ValueTask<PlayerBalanceInfoViewModel> PlayerBalanceInfoAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		string playerCode = "4ucmqplxb029",
		string gameProviderCode = "KY")
	{
		var result = await service.GetAsync(
			query: new PlayerBalanceQuery(
				PlayerCode: playerCode,
				GameProviderCode: gameProviderCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerBalanceInfoViewModel(
			result.Balance,
			result.FreeBalance,
			result.OutstandingBalance);
	}

	/// <summary>
	/// Players the log out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <returns></returns>
	[HttpDelete("kick/{playerCode}", Name = "Backoffice_Player_LogOut")]
	public ValueTask PlayerLogOutAsync(
		[FromServices] ICommandService<PlayerLogOutCommand> service,
		string playerCode = "4ucmqplxb029")
		=> service.ExecuteAsync(
			command: new PlayerLogOutCommand(
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the log out game provider asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <returns></returns>
	[HttpDelete("kick/{playerCode}/{gameProviderCode}", Name = "Backoffice_Player_LogOut_GameProvider")]
	public ValueTask PlayerLogOutGameProviderAsync(
		[FromServices] ICommandService<PlayerLogOutGameProviderCommand> service,
		string playerCode = "4ucmqplxb029",
		string gameProviderCode = "KY")
		=> service.ExecuteAsync(
			command: new PlayerLogOutGameProviderCommand(
				PlayerCode: playerCode,
				GameProviderCode: gameProviderCode),
			cancellationToken: HttpContext.RequestAborted);
}
