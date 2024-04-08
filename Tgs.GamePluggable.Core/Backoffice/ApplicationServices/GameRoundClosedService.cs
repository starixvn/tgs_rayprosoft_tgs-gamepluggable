using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundClosedService(
	ICashServiceClient cashServiceClient,
	IGameRoundsClient gameRoundsClient,
	IGameRecordHandleClient gameRecordHandleClient) : ICommandService<GameRoundClosedCommand>
{
	public async ValueTask ExecuteAsync(GameRoundClosedCommand command, CancellationToken cancellationToken = default)
	{
		var gameRoundModel = await gameRoundsClient.GetGameRoundAsync(
			roundId: command.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new Exception($"Game round number does not exist.({command.RoundId})");

		if (gameRoundModel.Status == GameRoundStatus.Closed)
		{
			throw new Exception($"Game round number has been closed.({command.RoundId})");
		}

		var wagerState = command.RoundClientStatus switch
		{
			GameRoundClientStatus.Win => WagerStates.Win,
			GameRoundClientStatus.Loss => WagerStates.Lose,
			GameRoundClientStatus.Void => WagerStates.Void,
			GameRoundClientStatus.Draw => WagerStates.Draw,
			_ => WagerStates.Running
		};

		JsonNode additionalData;
		try
		{
			additionalData = JsonNode.Parse(gameRoundModel.AdditionalData)!;
		}
		catch
		{
			additionalData = JsonNode.Parse("{}")!;
		}

		await gameRecordHandleClient.MessageAsync(
			gameProviderCode: gameRoundModel.GameProviderCode,
			playerCode: gameRoundModel.PlayerCode,
			wagerState: wagerState,
			externalRoundId: gameRoundModel.ExternalRoundId,
			externalWagerId: gameRoundModel.ExternalRoundId,
			externalGameId: gameRoundModel.ExternalGameId,
			externalCreatedOn: gameRoundModel.ExternalCreatedOn!.Value,
			externalClosedOn: command.ExternalClosedOn,
			bet: command.BetAmount,
			validBet: command.ValidBetAmount,
			settle: command.SettleAmount,
			commission: command.Commission,
			turnover: command.Turnover,
			winLoss: -command.CompanyWinLoss,
			additionalData: additionalData,
			cancellationToken: cancellationToken);

		if (command.AdjustAmount.HasValue)
		{
			_ = await cashServiceClient
				.UnsettledBetManualAsync(
				walletId: gameRoundModel.WalletId,
				amount: command.AdjustAmount.Value,
				description: command.Remark,
				externalId: gameRoundModel.RoundId.ToString(),
				createdByUserId: command.OperatorId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
