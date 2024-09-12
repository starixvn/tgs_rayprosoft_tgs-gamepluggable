using System;
using System.Data;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameBets.Client;
using Tgs.GameBets.RapidTest.Worker.Client;

using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundAddCommandService(
	ILogger<GameRoundAddCommandService> logger,
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IBrandGameProviderRepository brandGameProviderRepository,
	IGameBetsClient gameBetsClient,
	IGameBetsRapidTestWorkerClient gameBetsRapidTestWorkerClient) : ICommandService<GameRoundAddCommand>
{
	public async ValueTask ExecuteAsync(GameRoundAddCommand command, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(command.PlayerCode, cancellationToken).ConfigureAwait(false);
		var gameProviderInfo = await gameProviderRepository.GetAsync(command.GameProviderId, cancellationToken).ConfigureAwait(false);
		var gameInfo = await gameRepository.GetAsync(command.GameId, cancellationToken).ConfigureAwait(false);
		var gameProviderConfigurationId = await brandGameProviderRepository.GetGameProviderConfigurationIdAsync(playerInfo.BrandId, gameProviderInfo.GameProviderId, cancellationToken).ConfigureAwait(false);

		var gameBetInfo = await gameBetsClient.GameBetGetByExternalBetAsync(
			gameProviderId: gameProviderInfo.GameProviderId,
			gameId: gameInfo.GameId,
			walletId: playerInfo.WalletId,
			externalBetId: command.ExternalRoundId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (gameBetInfo is not null)
		{
			throw new DuplicateNameException($"This game bet already exists.( {new JsonObject
			{
				["GameProviderCode"] = gameProviderInfo.GameProviderCode,
				["PlayerCode"] = playerInfo.PlayerCode,
				["ExternalRoundId"] = gameBetInfo.ExternalRoundId,
				["ExternalGameId"] = gameInfo.ExternalGameId,
				["ExternalCreatedOn"] = command.ExternalCreatedOn,
				["ExternalClosedOn"] = command.ExternalClosedOn
			}.ToJsonString()})");
		}

		var betState = command.ClientStatus switch
		{
			GameRounds.Common.Enums.RoundClientStatus.Win => GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
			GameRounds.Common.Enums.RoundClientStatus.Loss => GameBets.RapidTest.Worker.Common.Protos.BetState.Loss,
			GameRounds.Common.Enums.RoundClientStatus.Void => GameBets.RapidTest.Worker.Common.Protos.BetState.Void,
			GameRounds.Common.Enums.RoundClientStatus.Draw => GameBets.RapidTest.Worker.Common.Protos.BetState.Draw,
			_ => GameBets.RapidTest.Worker.Common.Protos.BetState.Running
		};

		var source = new GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel
		{
			GameProviderId = gameProviderInfo.GameProviderId,
			GameProviderCode = gameProviderInfo.GameProviderCode,
			GameProviderConfigurationId = gameProviderConfigurationId,
			SubGameProviderCode = gameInfo.SubGameProviderCode,
			GameId = gameInfo.GameId,
			ResellerId = playerInfo.ResellerId,
			ResellerCode = playerInfo.ResellerCode,
			LicenseeId = playerInfo.LicenseeId,
			LicenseeCode = playerInfo.LicenseeCode,
			BrandId = playerInfo.BrandId,
			BrandCode = playerInfo.BrandCode,
			PlayerCode = playerInfo.PlayerCode,
			Username = playerInfo.Username,
			CurrencyCode = playerInfo.CurrencyCode,
			WalletId = playerInfo.WalletId,
			ExternalRoundId = command.ExternalRoundId,
			ExternalBetId = command.ExternalRoundId,
			ExternalGameId = gameInfo.ExternalGameId,
			ExternalCreatedOn = command.ExternalCreatedOn,
			ExternalClosedOn = command.ExternalClosedOn,
			ExternalPayoffOn = command.ExternalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = betState,
			BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
			FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
			GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
			CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
			ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
			JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
			CurrencyRatio = 1.0m,
			Bet = command.BetAmount,
			ValidBet = command.ValidBetAmount,
			Settle = command.SettleAmount,
			Commission = command.Commission,
			Turnover = command.Turnover,
			WinLoss = -command.CompanyWinLoss,
			Jackpot = 0.0M,
			AdditionalData = new JsonObject().ToJsonString()
		};

		await gameBetsRapidTestWorkerClient.HandleAsync(
			source: source,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogWarning("{logWarning}",
			new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					source.GameProviderCode,
					source.SubGameProviderCode,
					source.GameId,
					source.ResellerCode,
					source.LicenseeCode,
					source.BrandCode,
					source.PlayerCode,
					source.Username,
					source.CurrencyCode,
					source.WalletId,
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
