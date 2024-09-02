using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.GameRounds.Client;
using System.Data;
using Tgs.GameRecord.Handle.Client;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundAddCommandService(
	ILogger<GameRoundAddCommandService> logger,
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IGameRoundsClient gameRoundsClient,
	IGameRecordHandleClient gameRecordHandleClient) : ICommandService<GameRoundAddCommand>
{
	public async ValueTask ExecuteAsync(GameRoundAddCommand command, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderInfo = await gameProviderRepository.GetAsync(
			id: command.GameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameInfo = await gameRepository.GetAsync(
			id: command.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var round = await gameRoundsClient.RoundGetAsync(
			gameProviderId: gameProviderInfo.GameProviderId,
			gameId: gameInfo.GameId,
			walletId: playerInfo.WalletId,
			externalRoundId: command.ExternalRoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (round is not null)
		{
			throw new DuplicateNameException($"This game bet already exists.( {new JsonObject
			{
				["GameProviderCode"] = gameProviderInfo.GameProviderCode,
				["PlayerCode"] = playerInfo.PlayerCode,
				["ExternalRoundId"] = round.ExternalRoundId,
				["ExternalGameId"] = gameInfo.ExternalGameId,
				["ExternalCreatedOn"] = command.ExternalCreatedOn,
				["ExternalClosedOn"] = command.ExternalClosedOn
			}.ToJsonString()})");
		}

		var betState = command.ClientStatus switch
		{
			GameRounds.Common.Enums.RoundClientStatus.Win => GameRecord.Handle.Common.Protos.BetState.Win,
			GameRounds.Common.Enums.RoundClientStatus.Loss => GameRecord.Handle.Common.Protos.BetState.Loss,
			GameRounds.Common.Enums.RoundClientStatus.Void => GameRecord.Handle.Common.Protos.BetState.Void,
			GameRounds.Common.Enums.RoundClientStatus.Draw => GameRecord.Handle.Common.Protos.BetState.Draw,
			_ => GameRecord.Handle.Common.Protos.BetState.Running
		};

		var source = new GameRecord.Handle.Common.ViewModels.GameRecordHandleViewModel
		{
			GameProviderCode = gameProviderInfo.GameProviderCode,
			PlayerCode = command.PlayerCode,
			ExternalRoundId = command.ExternalRoundId,
			ExternalBetId = command.ExternalRoundId,
			ExternalGameId = gameInfo.ExternalGameId,
			ExternalCreatedOn = command.ExternalCreatedOn,
			ExternalClosedOn = command.ExternalClosedOn,
			ExternalPayoffOn = command.ExternalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = betState,
			BetType = GameRecord.Handle.Common.Protos.BetType.Normal,
			FreeGameState = GameRecord.Handle.Common.Protos.FreeGameState.None,
			GambleState = GameRecord.Handle.Common.Protos.GambleState.None,
			CashoutState = GameRecord.Handle.Common.Protos.CashoutState.None,
			ResettleState = GameRecord.Handle.Common.Protos.ResettleState.None,
			JackpotState = GameRecord.Handle.Common.Protos.JackpotState.None,
			Bet = command.BetAmount,
			ValidBet = command.ValidBetAmount,
			Settle = command.SettleAmount,
			Commission = command.Commission,
			Turnover = command.Turnover,
			WinLoss = -command.CompanyWinLoss,
			Jackpot = 0.0M,
			AdditionalData = JsonNode.Parse("{}")!.ToJsonString()
		};

		await gameRecordHandleClient.HandleAsync(
			source: source,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("{logWarning}",
			new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					source.GameProviderCode,
					source.PlayerCode,
					source.ExternalRoundId,
					source.ExternalBetId,
					source.ExternalGameId,
					ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
					ExternalClosedOn = source.ExternalClosedOn.ToString("u"),
					source.BetState
				}
			});
	}
}
