using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameRounds.Client;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundResettleCommandService(
	ILogger<GameRoundResettleCommandService> logger,
	ICashServiceClient cashServiceClient,
	IGameRoundsClient gameRoundsClient,
	IMessageSender messageSender) : ICommandService<GameRoundResettleCommand>
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

		var request = new GameRecord.Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = round.GameProviderCode,
			PlayerCode = round.PlayerCode,
			ExternalRoundId = round.ExternalRoundId,
			ExternalBetId = round.ExternalRoundId,
			ExternalGameId = round.ExternalGameId,
			ExternalCreatedOn = round.ExternalCreatedOn!.Value.ToTimestamp(),
			ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
			ExternalPayoffOn = command.ExternalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
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

		await messageSender.PublishAsync(
			subject: GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
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
