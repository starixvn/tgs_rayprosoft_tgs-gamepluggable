using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRounds.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundResettleCommandService(
	ILogger<GameRoundResettleCommandService> logger,
	ICashServiceClient cashServiceClient,
	IGameRoundsClient gameRoundsClient,
	IGameRecordHandleClient gameRecordHandleClient) : ICommandService<GameRoundResettleCommand>
{
	public async ValueTask ExecuteAsync(GameRoundResettleCommand command, CancellationToken cancellationToken = default)
	{
		var round = await gameRoundsClient.RoundGetAsync(
			roundId: command.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(command.RoundId.ToString(), $"This game bet does not exist.");

		if (round.Status != GameRounds.Common.Enums.RoundStatus.Closed)
		{
			throw new Exception($"This game's bet status is unsettled.({command.RoundId})");
		}

		var betState = command.ClientStatus switch
		{
			GameRounds.Common.Enums.RoundClientStatus.Win => GameRecord.Handle.Common.Protos.BetState.Win,
			GameRounds.Common.Enums.RoundClientStatus.Loss => GameRecord.Handle.Common.Protos.BetState.Loss,
			GameRounds.Common.Enums.RoundClientStatus.Void => GameRecord.Handle.Common.Protos.BetState.Void,
			GameRounds.Common.Enums.RoundClientStatus.Draw => GameRecord.Handle.Common.Protos.BetState.Draw,
			_ => GameRecord.Handle.Common.Protos.BetState.Running
		};

		JsonNode additionalData;
		try
		{
			additionalData = JsonNode.Parse(round.AdditionalData)!;
		}
		catch
		{
			additionalData = JsonNode.Parse("{}")!;
		}

		var source = new GameRecord.Handle.Common.ViewModels.GameRecordHandleViewModel
		{
			GameProviderCode = round.GameProviderCode,
			PlayerCode = round.PlayerCode,
			ExternalRoundId = round.ExternalRoundId,
			ExternalBetId = round.ExternalRoundId,
			ExternalGameId = round.ExternalGameId,
			ExternalCreatedOn = round.ExternalCreatedOn!.Value,
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
			AdditionalData = additionalData.ToJsonString()
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

		if (command.AdjustAmount.HasValue)
		{
			_ = await cashServiceClient.AdjustResettledBetManualAsync(
				walletId: round.WalletId,
				amount: command.AdjustAmount.Value,
				description: command.Remark,
				externalId: round.RoundId.ToString(),
				createdByUserId: command.OperatorId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
