using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class GameRoundResettleCommandServiceUnitTest
{
	[Fact]
	public async Task Can_Resettle_Closed_Game_Round()
	{
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
		var betAmount = 0.060000m;
		var validBetAmount = 0.060000m;
		var settleAmount = 0.00m;
		var commission = 0.0m;
		var turnover = 0.06m;
		var companyWinLoss = -0.06m;
		var roundClientStatus = GameRoundClientStatus.Loss;
		var externalClosedOn = DateTimeOffset.Parse("2021-06-03 13:57:05.000 +0800");
		var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
		var remark = "測試調整Closed Round";
		var adjustAmount = 100m;

		var command = new GameRoundResettleCommand(
			RoundId: roundId,
			BetAmount: betAmount,
			ValidBetAmount: validBetAmount,
			SettleAmount: settleAmount,
			Commission: commission,
			Turnover: turnover,
			CompanyWinLoss: companyWinLoss,
			RoundClientStatus: roundClientStatus,
			ExternalClosedOn: externalClosedOn,
			OperatorId: operatorId,
			Remark: remark,
			AdjustAmount: adjustAmount
		);

		var gameRoundModel = new GameRoundViewModel
		{
			RoundId = roundId,
			ExternalRoundId = "50-1636345294-40676339-2",
			GameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
			GameProviderCode = "EVO",
			PlayerCode = "4uf6j0t01809",
			GameId = 1000015,
			ExternalGameId = "bacc",
			CurrencyCode = "CNY",
			Status = GameRoundStatus.Open,
			GameProviderConfigurationId = Guid.Parse("fa0d7706-f910-4e98-bf71-bd7084140304"),
			WalletId = Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9"),
			CreatedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			ClosedOn = null,
			UpdatedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			BrandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			ExternalClosedOn = null,
			BetAmount = 0.060000m,
			ValidBetAmount = 0.060000m,
			SettleAmount = 0m,
			Commission = 0m,
			Turnover = 0m,
			CompanyWinLoss = 0m,
			TieBetAdjustment = 0m,
			ClientStatus = GameRoundClientStatus.Open,
			ReopenTurnoverAdjustment = null,
			ReopenedOn = null,
			EffectiveBettingPercentage = null,
			AdditionalData = "{}"
		};

		_ = fakeGameRoundsClient
			.GetGameRoundAsync(roundId)
			.Returns(gameRoundModel);

		var sut = new GameRoundResettleCommandService(
			fakeCashServiceClient,
			fakeGameRoundsClient,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		var wagerState = command.RoundClientStatus switch
		{
			GameRoundClientStatus.Win => WagerStates.Win,
			GameRoundClientStatus.Loss => WagerStates.Lose,
			GameRoundClientStatus.Void => WagerStates.Void,
			GameRoundClientStatus.Draw => WagerStates.Draw,
			_ => WagerStates.Running
		};

		await fakeGameRecordHandleClient
			.Received(1)
			.MessageAsync(
			gameProviderCode: Arg.Is(gameRoundModel.GameProviderCode),
			playerCode: Arg.Is(gameRoundModel.PlayerCode),
			wagerState: Arg.Is(wagerState),
			externalRoundId: Arg.Is(gameRoundModel.ExternalRoundId),
			externalWagerId: Arg.Is(gameRoundModel.ExternalRoundId),
			externalGameId: Arg.Is(gameRoundModel.ExternalGameId),
			externalCreatedOn: Arg.Is(gameRoundModel.ExternalCreatedOn!.Value),
			externalClosedOn: Arg.Is(command.ExternalClosedOn),
			bet: Arg.Is(command.BetAmount),
			validBet: Arg.Is(command.ValidBetAmount),
			settle: Arg.Is(command.SettleAmount),
			commission: Arg.Is(command.Commission),
			turnover: Arg.Is(command.Turnover),
			winLoss: Arg.Is(-command.CompanyWinLoss),
			additionalData: Arg.Any<JsonNode>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(1)
			.AdjustResettledBetManualAsync(
			walletId: Arg.Is(gameRoundModel.WalletId),
			amount: Arg.Is(adjustAmount),
			description: Arg.Is(remark),
			externalId: Arg.Is(roundId.ToString()),
			createdByUserId: Arg.Is(operatorId),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
