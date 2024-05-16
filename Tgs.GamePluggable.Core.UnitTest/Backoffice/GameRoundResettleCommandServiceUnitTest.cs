using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameRounds.Client;
using Valhalla.MessageQueue;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class GameRoundResettleCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
		var betAmount = 0.060000m;
		var validBetAmount = 0.060000m;
		var settleAmount = 0.120000m;
		var commission = 0.0m;
		var turnover = 0.06m;
		var companyWinLoss = -0.06m;
		var clientStatus = GameRounds.Common.Enums.RoundClientStatus.Win;
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
			ClientStatus: clientStatus,
			ExternalClosedOn: externalClosedOn,
			OperatorId: operatorId,
			Remark: remark,
			AdjustAmount: adjustAmount
		);

		var round = new GameRounds.Common.ViewModels.RoundViewModel
		{
			RoundId = roundId,
			ExternalRoundId = "50-1636345294-40676339-2",
			GameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
			GameProviderCode = "EVO",
			PlayerCode = "4uf6j0t01809",
			GameId = 1000015,
			ExternalGameId = "bacc",
			CurrencyCode = "CNY",
			Status = GameRounds.Common.Enums.RoundStatus.Closed,
			GameProviderConfigurationId = Guid.Parse("fa0d7706-f910-4e98-bf71-bd7084140304"),
			WalletId = Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9"),
			CreatedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			ClosedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			UpdatedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			BrandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			ExternalClosedOn = DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
			BetAmount = 0.060000m,
			ValidBetAmount = 0.060000m,
			SettleAmount = 0.00000m,
			Commission = 0.00000m,
			Turnover = 0.060000m,
			CompanyWinLoss = 0.060000m,
			ClientStatus = GameRounds.Common.Enums.RoundClientStatus.Loss,
			AdditionalData = "{}"
		};

		_ = fakeGameRoundsClient.RoundGetAsync(
			roundId: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(round));

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakeCashServiceClient,
			fakeGameRoundsClient,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new GameRecord.Handle.Common.Protos.GameRecordHandleRequest
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
				BetState = GameRecord.Handle.Common.Protos.BetState.Win,
				BetType = GameRecord.Handle.Common.Protos.BetType.Normal,
				FreeGameState = GameRecord.Handle.Common.Protos.FreeGameState.None,
				GambleState = GameRecord.Handle.Common.Protos.GambleState.None,
				CashoutState = GameRecord.Handle.Common.Protos.CashoutState.None,
				ResettleState = GameRecord.Handle.Common.Protos.ResettleState.None,
				JackpotState = GameRecord.Handle.Common.Protos.JackpotState.None,
				Bet = 0.060000m,
				ValidBet = 0.060000m,
				Settle = 0.120000m,
				Commission = 0.00000m,
				Turnover = 0.060000m,
				WinLoss = 0.060000m,
				Jackpot = 0.0M,
				AdditionalData = round.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(1)
			.AdjustResettledBetManualAsync(
			walletId: Arg.Is(round.WalletId),
			amount: Arg.Is(adjustAmount),
			description: Arg.Is(remark),
			externalId: Arg.Is(roundId.ToString()),
			createdByUserId: Arg.Is(operatorId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_games_bet_status_is_unsettled()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
		var betAmount = 0.060000m;
		var validBetAmount = 0.060000m;
		var settleAmount = 0.120000m;
		var commission = 0.0m;
		var turnover = 0.06m;
		var companyWinLoss = -0.06m;
		var clientStatus = GameRounds.Common.Enums.RoundClientStatus.Win;
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
			ClientStatus: clientStatus,
			ExternalClosedOn: externalClosedOn,
			OperatorId: operatorId,
			Remark: remark,
			AdjustAmount: adjustAmount
		);

		var round = new GameRounds.Common.ViewModels.RoundViewModel
		{
			RoundId = roundId,
			ExternalRoundId = "50-1636345294-40676339-2",
			GameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
			GameProviderCode = "EVO",
			PlayerCode = "4uf6j0t01809",
			GameId = 1000015,
			ExternalGameId = "bacc",
			CurrencyCode = "CNY",
			Status = GameRounds.Common.Enums.RoundStatus.Open,
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
			SettleAmount = 0.00000m,
			Commission = 0.00000m,
			Turnover = 0.00000m,
			CompanyWinLoss = 0.00000m,
			ClientStatus = GameRounds.Common.Enums.RoundClientStatus.Open,
			AdditionalData = "{}"
		};

		_ = fakeGameRoundsClient.RoundGetAsync(
			roundId: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(round));

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakeCashServiceClient,
			fakeGameRoundsClient,
			fakeMessageSender);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Any<GameRecord.Handle.Common.Protos.GameRecordHandleRequest>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.UnsettledBetManualAsync(
			walletId: Arg.Any<Guid>(),
			amount: Arg.Any<decimal>(),
			description: Arg.Any<string>(),
			externalId: Arg.Any<string>(),
			createdByUserId: Arg.Any<Guid>());
	}

	[Fact]
	public async Task This_game_bet_does_not_exist()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
		var betAmount = 0.060000m;
		var validBetAmount = 0.060000m;
		var settleAmount = 0.120000m;
		var commission = 0.0m;
		var turnover = 0.06m;
		var companyWinLoss = -0.06m;
		var clientStatus = GameRounds.Common.Enums.RoundClientStatus.Win;
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
			ClientStatus: clientStatus,
			ExternalClosedOn: externalClosedOn,
			OperatorId: operatorId,
			Remark: remark,
			AdjustAmount: adjustAmount
		);

		_ = fakeGameRoundsClient.RoundGetAsync(
			roundId: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameRounds.Common.ViewModels.RoundViewModel)null);

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakeCashServiceClient,
			fakeGameRoundsClient,
			fakeMessageSender);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Any<GameRecord.Handle.Common.Protos.GameRecordHandleRequest>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.UnsettledBetManualAsync(
			walletId: Arg.Any<Guid>(),
			amount: Arg.Any<decimal>(),
			description: Arg.Any<string>(),
			externalId: Arg.Any<string>(),
			createdByUserId: Arg.Any<Guid>());
	}
}
