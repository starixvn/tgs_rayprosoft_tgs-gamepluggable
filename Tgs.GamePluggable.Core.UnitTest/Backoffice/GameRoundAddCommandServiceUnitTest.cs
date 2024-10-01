using System;
using System.Data;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

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
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeGameBetsRapidTestWorkerClient = Substitute.For<IGameBetsRapidTestWorkerClient>();

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
			ClientStatus: Enums.RoundClientStatus.Win,
			OperatorId: Guid.NewGuid(),
			Remark: "手動建立");

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
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
			AvailableCurrencies: null,
			SubGameProviderCode: gameProviderInfo.GameProviderCode);
		_ = fakeGameRepository.GetAsync(
			Arg.Is(command.GameId),
			Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			Arg.Is(playerInfo.BrandId),
			Arg.Is(gameProviderInfo.GameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		_ = fakeGameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameId: Arg.Is(gameInfo.GameId),
			walletId: Arg.Is(playerInfo.WalletId),
			externalBetId: Arg.Is(command.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBets.Common.ViewModels.GameBetInfoViewModel>(null));

		var sut = new GameRoundAddCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeBrandGameProviderRepository,
			fakeGameBetsClient,
			fakeGameBetsRapidTestWorkerClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetsRapidTestWorkerClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel
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
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
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
				WinLoss = -command.CompanyWinLoss,
				Turnover = command.Turnover,
				Commission = command.Commission,
				Contribute = 0.0m,
				Jackpot = 0.0m,
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
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeGameBetsRapidTestWorkerClient = Substitute.For<IGameBetsRapidTestWorkerClient>();

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
			ClientStatus: Enums.RoundClientStatus.Win,
			OperatorId: Guid.NewGuid(),
			Remark: "手動建立");

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
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
			AvailableCurrencies: null,
			SubGameProviderCode: gameProviderInfo.GameProviderCode);
		_ = fakeGameRepository.GetAsync(
			Arg.Is(command.GameId),
			Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			Arg.Is(playerInfo.BrandId),
			Arg.Is(gameProviderInfo.GameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameBetInfo = new GameBets.Common.ViewModels.GameBetInfoViewModel(
			Id: Guid.NewGuid(),
			GameProviderId: gameProviderInfo.GameProviderId,
			GameProviderConfigurationId: gameProviderConfigurationId,
			GameId: gameInfo.GameId,
			ResellerId: playerInfo.ResellerId,
			LicenseeId: playerInfo.LicenseeId,
			BrandId: playerInfo.BrandId,
			PlayerCode: playerInfo.PlayerCode,
			CurrencyCode: playerInfo.CurrencyCode,
			WalletId: playerInfo.WalletId,
			ExternalRoundId: command.ExternalRoundId,
			ExternalBetId: command.ExternalRoundId,
			ExternalCreatedOn: command.ExternalCreatedOn,
			ExternalClosedOn: command.ExternalClosedOn,
			ExternalPayoffOn: command.ExternalClosedOn,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 1.0m,
			ValidBet: 1.0m,
			Settle: 2.0m,
			WinLoss: 1.0m,
			Turnover: 1.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject { },
			ExtendedData: new JsonObject { });
		_ = fakeGameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameId: Arg.Is(gameInfo.GameId),
			walletId: Arg.Is(playerInfo.WalletId),
			externalBetId: Arg.Is(command.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GameRoundAddCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeBrandGameProviderRepository,
			fakeGameBetsClient,
			fakeGameBetsRapidTestWorkerClient);

		_ = await Assert.ThrowsAsync<DuplicateNameException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeGameBetsRapidTestWorkerClient
			.Received(0)
			.HandleAsync(
			source: Arg.Any<GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
