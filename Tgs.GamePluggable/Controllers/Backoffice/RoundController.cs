using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Controllers.Backoffice;

[Route("bo/[controller]")]
[ApiController]
public class RoundController : ControllerBase
{
	/// <summary>
	/// Adds the company round asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPut("addCompanyRound")]
	public ValueTask AddCompanyRoundAsync(
		[FromServices] ICommandService<GameRoundAddCommand> service,
		[FromBody] AddCompanyRoundViewModel source)
	=> service.ExecuteAsync(
		command: new GameRoundAddCommand(
			ExternalRoundId: source.ExternalRoundId,
			GameProviderId: source.GameProviderId,
			PlayerCode: source.PlayerCode,
			GameId: source.GameId,
			CurrencyCode: source.CurrencyCode,
			WalletId: source.WalletId,
			BrandId: source.BrandId,
			ExternalCreatedOn: source.ExternalCreatedOn,
			ExternalClosedOn: source.ExternalClosedOn,
			BetAmount: source.BetAmount,
			ValidBetAmount: source.ValidBetAmount,
			SettleAmount: source.SettleAmount,
			Turnover: source.Turnover,
			Commission: source.Commission,
			CompanyWinLoss: source.CompanyWinLoss,
			ClientStatus: (GameRounds.Common.Enums.RoundClientStatus)source.ClientRoundStatus,
			OperatorId: source.OperatorId,
			Remark: source.Remark),
		cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Adjusts the closed game round asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="roundId">The round identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPut("{roundId}/adjustclosedRound")]
	public ValueTask AdjustClosedGameRoundAsync(
		[FromServices] ICommandService<GameRoundResettleCommand> service,
		Guid roundId,
		[FromBody] AdjustClosedRoundViewModel source)
		=> service.ExecuteAsync(
			command: new GameRoundResettleCommand(
				RoundId: roundId,
				BetAmount: source.BetAmount,
				ValidBetAmount: source.ValidBetAmount,
				SettleAmount: source.SettleAmount,
				Commission: source.Commission,
				Turnover: source.Turnover,
				CompanyWinLoss: source.WinLoss,
				ClientStatus: (GameRounds.Common.Enums.RoundClientStatus)source.ClientRoundStatus,
				ExternalClosedOn: source.ExternalClosedOn,
				OperatorId: source.OperatorId,
				Remark: source.Remark,
				AdjustAmount: source.AdjustAmount),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the action query asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="roundId">The round identifier.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     GetAsync /bo/round/e8a90f53-11e7-40ab-9ed3-08d99f799e5a/GameActionQuery
	/// </remarks>
	[HttpGet("{roundId}/gameactionquery")]
	public IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(
		[FromServices] IQueryService<GameActionQuery, GameActionResult> service,
		Guid roundId)
	{
		var query = new GameActionQuery(
			RoundId: roundId);

		return service.QueryAsync(query)
			.Select(data => new GameActionViewModel(
				Id: data.Id,
				RoundId: data.RoundId,
				PlayerCode: data.PlayerCode,
				ExternalTransactionId: data.ExternalTransactionId,
				Type: (GameActionViewModelType)data.Type,
				Amount: data.Amount,
				Turnover: data.Turnover,
				WinLoss: data.WinLoss,
				UnsettledBets: data.UnsettledBets,
				Balance: data.Balance,
				BetType: data.BetType,
				ExternalBetId: data.ExternalBetId,
				ExternalGameActionGroupId: data.ExternalGameActionGroupId,
				ReferencedGameActionId: data.ReferencedGameActionId,
				GameActionBatchId: data.GameActionBatchId,
				RecordedOn: data.RecordedOn,
				SentOn: data.SentOn,
				IsClosingRound: data.IsClosingRound,
				ClosingRoundExplicitRequest: data.ClosingRoundExplicitRequest,
				PlayerIp: data.PlayerIp,
				ExtraData: data.ExtraData,
				JackpotContribution: data.JackpotContribution,
				StoredOn: data.StoredOn));
	}

	/// <summary>
	/// Games the round closed asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="roundId">The round identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Put /bo/round/15426ced-d81b-42a6-a0a3-08d971a93a52/ClosedGameRound
	///     {
	///         "betAmount": 100,
	///         "validBetAmount": 0,
	///         "settleAmount": 0,
	///         "commission": 0,
	///         "turnover": 0,
	///         "winLoss": 0,
	///         "clientRoundStatus": 2,
	///         "externalClosedOn": "2021-09-07T17:40:24.000+08:00",
	///         "operatorId": "7acf882a-474b-ec11-bf75-8c554ae8ab6b",
	///         "remark": "測試關閉",
	///         "adjustAmount": 100
	///     }
	/// </remarks>
	[HttpPut("{roundId}/gameroundclosed")]
	public ValueTask GameRoundClosedAsync(
		[FromServices] ICommandService<GameRoundClosedCommand> service,
		Guid roundId,
		[FromBody] GameRoundClosedViewModel source)
		=> service.ExecuteAsync(
			command: new GameRoundClosedCommand(
				RoundId: roundId,
				BetAmount: source.BetAmount,
				ValidBetAmount: source.ValidBetAmount,
				SettleAmount: source.SettleAmount,
				Commission: source.Commission,
				Turnover: source.Turnover,
				CompanyWinLoss: source.WinLoss,
				ClientStatus: (GameRounds.Common.Enums.RoundClientStatus)source.ClientRoundStatus,
				ExternalClosedOn: source.ExternalClosedOn,
				OperatorId: source.OperatorId,
				Remark: source.Remark,
				AdjustAmount: source.AdjustAmount),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the round detail get asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="roundId">The round identifier.</param>
	/// <param name="language">The language.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     GetAsync /bo/round/3c6282df-d531-4482-088e-08d9a004e811/gamerounddetail?language=zh-CN
	/// </remarks>
	[HttpGet("{roundId}/gamerounddetail")]
	public async ValueTask<JsonDocument> GameRoundDetailGetAsync(
		[FromServices] IGetService<RoundDetailQuery, JsonDocument> service,
		Guid roundId,
		[FromQuery] string language)
		=> await service.GetAsync(
			new()
			{
				RoundId = roundId,
				Language = language
			})
			.ConfigureAwait(false);

	/// <summary>
	/// Games the round get asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="roundId">The round identifier.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     GetAsync /bo/round/e8a90f53-11e7-40ab-9ed3-08d99f799e5a
	/// </remarks>
	[HttpGet("{roundId}")]
	public async ValueTask<GameRoundViewModel> GameRoundGetAsync(
		[FromServices] IGetService<GameRoundQuery, GameRoundResult> service,
		Guid roundId)
	{
		var query = new GameRoundQuery(
			RoundId: roundId);
		var data = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameRoundViewModel()
		{
			RoundId = data.RoundId,
			ExternalRoundId = data.ExternalRoundId,
			GameProviderId = data.GameProviderId,
			PlayerCode = data.PlayerCode,
			GameId = data.GameId,
			CurrencyCode = data.CurrencyCode,
			Status = (int)data.Status,
			GameProviderConfigurationId = data.GameProviderConfigurationId,
			WalletId = data.WalletId,
			CreatedOn = data.CreatedOn,
			ClosedOn = data.ClosedOn,
			UpdatedOn = data.UpdatedOn,
			BrandId = data.BrandId,
			ExternalCreatedOn = data.ExternalCreatedOn,
			ExternalClosedOn = data.ExternalClosedOn,
			BetAmount = data.BetAmount,
			ValidBetAmount = data.ValidBetAmount,
			SettleAmount = data.SettleAmount,
			Commission = data.Commission,
			Turnover = data.Turnover,
			WinLoss = data.WinLoss,
			TieBetAdjustment = data.TieBetAdjustment,
			ClientStatus = (int)data.ClientStatus
		};
	}
}
