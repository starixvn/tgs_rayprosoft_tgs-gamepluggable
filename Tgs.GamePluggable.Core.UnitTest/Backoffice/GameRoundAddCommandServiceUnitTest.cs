using System;
using System.Data;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRounds.Client;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class GameRoundAddCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<GameRoundAddCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new GameRoundAddCommand(
			ExternalRoundId: Guid.NewGuid().ToString("N"),
			GameProviderId: Guid.NewGuid(),
			PlayerCode: "4uf6j0t01809",
			GameId: 1,
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			ExternalCreatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09 +0800"),
			ExternalClosedOn: DateTimeOffset.Parse("2021-11-08 12:25:30 +0800"),
			BetAmount: 10,
			ValidBetAmount: 5,
			SettleAmount: 4.75m,
			Commission: 0.0m,
			Turnover: 5,
			CompanyWinLoss: 0.25m,
			ClientStatus: GameRounds.Common.Enums.RoundClientStatus.Win,
			OperatorId: Guid.NewGuid(),
			Remark: "手動建立");

		var playerInfo = new Models.PlayerInfo(
			BrandId: command.BrandId,
			BrandCode: "GPI",
			PlayerCode: "4uf6j0t01809",
			Username: "4uf6j0t01809",
			WalletId: command.WalletId,
			CurrencyCode: command.CurrencyCode);
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderInfo = new Models.GameProviderInfo(
			GameProviderId: command.GameProviderId,
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(command.GameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var gameInfo = new Models.GameInfo(
			GameId: command.GameId,
			GameCode: "ky_baccarat",
			GameProviderId: gameProviderInfo.GameProviderId,
			GameProviderCode: gameProviderInfo.GameProviderCode,
			ExternalGameId: "baccarat",
			Type: Enums.GameType.CardGame,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: string.Empty,
			RuleUrl: string.Empty,
			AvailableCurrencies: null);
		_ = fakeGameRepository.GetAsync(
			Arg.Is(command.GameId),
			Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		_ = fakeGameRoundsClient.RoundGetAsync(
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameId: Arg.Is(gameInfo.GameId),
			walletId: Arg.Is(playerInfo.WalletId),
			externalRoundId: Arg.Is(command.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameRounds.Common.ViewModels.RoundViewModel)null);

		var sut = new GameRoundAddCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameRoundsClient,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new GameRecord.Handle.Common.ViewModels.GameRecordHandleViewModel
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
				BetState = GameRecord.Handle.Common.Protos.BetState.Win,
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
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_already_exists()
	{
		var faceLogger = NullLogger<GameRoundAddCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new GameRoundAddCommand(
			ExternalRoundId: Guid.NewGuid().ToString("N"),
			GameProviderId: Guid.NewGuid(),
			PlayerCode: "4uf6j0t01809",
			GameId: 1,
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			ExternalCreatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09 +0800"),
			ExternalClosedOn: DateTimeOffset.Parse("2021-11-08 12:25:30 +0800"),
			BetAmount: 10,
			ValidBetAmount: 5,
			SettleAmount: 4.75m,
			Commission: 0.0m,
			Turnover: 5,
			CompanyWinLoss: 0.25m,
			ClientStatus: GameRounds.Common.Enums.RoundClientStatus.Win,
			OperatorId: Guid.NewGuid(),
			Remark: "手動建立");

		var playerInfo = new Models.PlayerInfo(
			BrandId: command.BrandId,
			BrandCode: "GPI",
			PlayerCode: "4uf6j0t01809",
			Username: "4uf6j0t01809",
			WalletId: command.WalletId,
			CurrencyCode: command.CurrencyCode);
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderInfo = new Models.GameProviderInfo(
			GameProviderId: command.GameProviderId,
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(command.GameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var gameInfo = new Models.GameInfo(
			GameId: command.GameId,
			GameCode: "ky_baccarat",
			GameProviderId: gameProviderInfo.GameProviderId,
			GameProviderCode: gameProviderInfo.GameProviderCode,
			ExternalGameId: "baccarat",
			Type: Enums.GameType.CardGame,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: string.Empty,
			RuleUrl: string.Empty,
			AvailableCurrencies: null);
		_ = fakeGameRepository.GetAsync(
			Arg.Is(command.GameId),
			Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var round = new GameRounds.Common.ViewModels.RoundViewModel
		{
			RoundId = Guid.NewGuid(),
			ExternalRoundId = command.ExternalRoundId,
			GameProviderId = gameProviderInfo.GameProviderId,
			GameProviderCode = gameProviderInfo.GameProviderCode,
			PlayerCode = playerInfo.PlayerCode,
			GameId = gameInfo.GameId,
			ExternalGameId = gameInfo.ExternalGameId,
			CurrencyCode = playerInfo.CurrencyCode,
			Status = GameRounds.Common.Enums.RoundStatus.Closed,
			GameProviderConfigurationId = Guid.NewGuid(),
			WalletId = playerInfo.WalletId,
			CreatedOn = DateTimeOffset.UtcNow,
			ClosedOn = DateTimeOffset.UtcNow,
			UpdatedOn = DateTimeOffset.UtcNow,
			BrandId = playerInfo.BrandId,
			ExternalCreatedOn = command.ExternalCreatedOn,
			ExternalClosedOn = command.ExternalClosedOn,
			BetAmount = command.BetAmount,
			ValidBetAmount = command.ValidBetAmount,
			SettleAmount = command.SettleAmount,
			Commission = command.Commission,
			Turnover = command.Turnover,
			CompanyWinLoss = command.CompanyWinLoss,
			AdditionalData = "{}"
		};
		_ = fakeGameRoundsClient.RoundGetAsync(
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameId: Arg.Is(gameInfo.GameId),
			walletId: Arg.Is(playerInfo.WalletId),
			externalRoundId: Arg.Is(command.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(round);

		var sut = new GameRoundAddCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameRoundsClient,
			fakeGameRecordHandleClient);

		_ = await Assert.ThrowsAsync<DuplicateNameException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeGameRecordHandleClient
			.Received(0)
			.HandleAsync(
			source: Arg.Any<GameRecord.Handle.Common.ViewModels.GameRecordHandleViewModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
