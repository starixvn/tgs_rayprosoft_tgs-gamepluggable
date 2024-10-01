using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameBets.Client;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundResettleCommandService(
	ILogger<GameRoundResettleCommandService> logger,
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IGameBetsClient gameBetsClient,
	IGameBetsRapidTestWorkerClient gameBetsRapidTestWorkerClient,
	ICashServiceClient cashServiceClient) : ICommandService<GameRoundResettleCommand>
{
	public async ValueTask ExecuteAsync(GameRoundResettleCommand command, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: command.ExternalClosedOn.AddDays(-45),
			id: command.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(command.RoundId.ToString(), $"This game bet does not exist.");

		if (gameBetInfo.BetState is GameBets.Common.Enums.BetState.Waiting or GameBets.Common.Enums.BetState.Running)
		{
			throw new Exception($"This game's bet status has been unsettled.({command.RoundId})");
		}

		var betState = command.ClientStatus switch
		{
			Enums.RoundClientStatus.Win => GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
			Enums.RoundClientStatus.Loss => GameBets.RapidTest.Worker.Common.Protos.BetState.Loss,
			Enums.RoundClientStatus.Void => GameBets.RapidTest.Worker.Common.Protos.BetState.Void,
			Enums.RoundClientStatus.Draw => GameBets.RapidTest.Worker.Common.Protos.BetState.Draw,
			_ => GameBets.RapidTest.Worker.Common.Protos.BetState.Running
		};

		var playerInfo = await playerRepository.GetAsync(gameBetInfo.PlayerCode, cancellationToken).ConfigureAwait(false);
		var gameProviderInfo = await gameProviderRepository.GetAsync(gameBetInfo.GameProviderId, cancellationToken).ConfigureAwait(false);
		var gameInfo = await gameRepository.GetAsync(gameBetInfo.GameId, cancellationToken).ConfigureAwait(false);

		var source = new GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel
		{
			GameProviderId = gameProviderInfo.GameProviderId,
			GameProviderCode = gameProviderInfo.GameProviderCode,
			GameProviderConfigurationId = gameBetInfo.GameProviderConfigurationId,
			SubGameProviderCode = gameInfo.SubGameProviderCode,
			GameId = gameBetInfo.GameId,
			ResellerId = gameBetInfo.ResellerId,
			ResellerCode = playerInfo.ResellerCode,
			LicenseeId = gameBetInfo.LicenseeId,
			LicenseeCode = playerInfo.LicenseeCode,
			BrandId = gameBetInfo.BrandId,
			BrandCode = playerInfo.BrandCode,
			PlayerCode = gameBetInfo.PlayerCode,
			Username = playerInfo.Username,
			CurrencyCode = gameBetInfo.CurrencyCode,
			WalletId = gameBetInfo.WalletId,
			ExternalRoundId = gameBetInfo.ExternalRoundId,
			ExternalBetId = gameBetInfo.ExternalBetId,
			ExternalGameId = gameInfo.ExternalGameId,
			ExternalCreatedOn = gameBetInfo.ExternalCreatedOn,
			ExternalClosedOn = command.ExternalClosedOn,
			ExternalPayoffOn = command.ExternalClosedOn,
			ExternalResettleOn = gameBetInfo.ExternalResettleOn,
			BetState = betState,
			BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
			FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
			GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
			CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
			ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
			JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
			CurrencyRatio = gameBetInfo.CurrencyRatio,
			Bet = command.BetAmount,
			ValidBet = command.ValidBetAmount,
			Settle = command.SettleAmount,
			WinLoss = -command.CompanyWinLoss,
			Turnover = command.Turnover,
			Commission = command.Commission,
			Contribute = gameBetInfo.Contribute,
			Jackpot = gameBetInfo.Jackpot,
			AdditionalData = gameBetInfo.AdditionalData.ToJsonString()
		};
		;

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
					source.PlayerCode,
					source.ExternalRoundId,
					source.ExternalBetId,
					source.ExternalGameId,
					ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
					ExternalClosedOn = source.ExternalClosedOn.ToString("u"),
					source.BetState
				}
			});

		if (command.AdjustAmount.HasValue)
		{
			_ = await cashServiceClient.AdjustResettledBetManualAsync(
				walletId: gameBetInfo.WalletId,
				amount: command.AdjustAmount.Value,
				description: command.Remark,
				externalId: gameBetInfo.Id.ToString(),
				createdByUserId: command.OperatorId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
