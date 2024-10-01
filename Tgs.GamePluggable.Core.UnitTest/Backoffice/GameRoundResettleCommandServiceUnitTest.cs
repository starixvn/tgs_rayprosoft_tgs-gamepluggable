using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameBets.Client;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class GameRoundResettleCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeGameBetsRapidTestWorkerClient = Substitute.For<IGameBetsRapidTestWorkerClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var roundId = Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552");
		var betAmount = 0.8m;
		var validBetAmount = 0.8m;
		var settleAmount = 1.2m;
		var commission = 0.0m;
		var turnover = 0.8m;
		var companyWinLoss = 0.4m;
		var clientStatus = Enums.RoundClientStatus.Win;
		var externalClosedOn = DateTimeOffset.Parse("2024-09-12T09:55:15+08:00");
		var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
		var remark = "測試Resettle調整";
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

		var gameBetInfo = new GameBets.Common.ViewModels.GameBetInfoViewModel(
			Id: Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552"),
			GameProviderId: Guid.Parse("147cef56-4fcd-4ee3-b748-87ad398f2871"),
			GameProviderConfigurationId: Guid.Parse("dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a"),
			GameId: 6700018,
			ResellerId: Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
			LicenseeId: Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
			BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4unykr72x8d9",
			CurrencyCode: "CNY",
			WalletId: Guid.Parse("8bff49a2-66ff-4c66-81f6-c9b242f2b868"),
			ExternalRoundId: "6323442723",
			ExternalBetId: "6323442723-01",
			ExternalCreatedOn: DateTimeOffset.Parse("2024-09-12T09:55:12+08:00"),
			ExternalClosedOn: DateTimeOffset.Parse("2024-09-12T09:55:15+08:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("2024-09-12T09:55:15+08:00"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 0.8m,
			ValidBet: 0.8m,
			Settle: 0.0m,
			WinLoss: -0.8m,
			Turnover: 0.8m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("{\"bet\": 0.8, \"win\": 1.2, \"rake\": 0, \"round\": \"6323442723\", \"detail\": [{\"freegame\": 0}, {\"luckydraw\": 0}, {\"bonus\": 0}], \"donate\": 0, \"status\": \"complete\", \"account\": \"4unykr72x8d9\", \"balance\": 84.4, \"bettime\": \"2024-09-11T21:55:12.737-04:00\", \"bettype\": [], \"jackpot\": 0, \"roomfee\": 0, \"tableid\": \"\", \"currency\": \"CNY\", \"gamecode\": \"20\", \"gamehall\": \"cq9\", \"gameplat\": \"web\", \"gamerole\": \"\", \"gametype\": \"slot\", \"isdonate\": false, \"validbet\": 0, \"tabletype\": \"\", \"bankertype\": \"\", \"createtime\": \"2024-09-11T21:55:15.456-04:00\", \"gameresult\": {\"cards\": [], \"points\": []}, \"jackpottype\": \"\", \"roundnumber\": \"\", \"endroundtime\": \"2024-09-11T21:55:15.555-04:00\", \"singlerowbet\": false, \"jackpotcontribution\": []}"),
			ExtendedData: new JsonObject { });
		_ = fakeGameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: Arg.Is(command.ExternalClosedOn.AddDays(-45)),
			id: Arg.Is(command.RoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: gameBetInfo.ResellerId,
			ResellerCode: "DEMO",
			LicenseeId: gameBetInfo.LicenseeId,
			LicenseeCode: "TGS",
			BrandId: gameBetInfo.BrandId,
			BrandCode: "GPI",
			PlayerCode: "4unykr72x8d9",
			Username: "4unykr72x8d9",
			WalletId: gameBetInfo.WalletId,
			CurrencyCode: gameBetInfo.CurrencyCode);
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(gameBetInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderInfo = new Models.GameProviderInfo(
			GameProviderId: gameBetInfo.GameProviderId,
			GameProviderCode: "CQ9",
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameBetInfo.GameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var gameInfo = new Models.GameInfo(
			GameId: gameBetInfo.GameId,
			GameCode: "888",
			GameProviderId: gameProviderInfo.GameProviderId,
			GameProviderCode: gameProviderInfo.GameProviderCode,
			ExternalGameId: "20",
			Type: Enums.GameType.Slot,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: string.Empty,
			RuleUrl: string.Empty,
			AvailableCurrencies: null,
			SubGameProviderCode: gameProviderInfo.GameProviderCode);
		_ = fakeGameRepository.GetAsync(
			Arg.Is(gameBetInfo.GameId),
			Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameBetsClient,
			fakeGameBetsRapidTestWorkerClient,
			fakeCashServiceClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameBetsRapidTestWorkerClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is<GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel>(compare =>
			compare.GameProviderId == gameProviderInfo.GameProviderId &&
				compare.GameProviderCode == gameProviderInfo.GameProviderCode &&
				compare.GameProviderConfigurationId == gameBetInfo.GameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameBetInfo.GameId &&
				compare.ResellerId == gameBetInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == gameBetInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == gameBetInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == gameBetInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == gameBetInfo.CurrencyCode &&
				compare.WalletId == gameBetInfo.WalletId &&
				compare.ExternalRoundId == gameBetInfo.ExternalRoundId &&
				compare.ExternalBetId == gameBetInfo.ExternalBetId &&
				compare.ExternalGameId == gameInfo.ExternalGameId &&
				compare.ExternalCreatedOn == gameBetInfo.ExternalCreatedOn &&
				compare.ExternalClosedOn == command.ExternalClosedOn &&
				compare.ExternalPayoffOn == command.ExternalClosedOn &&
				compare.ExternalResettleOn == gameBetInfo.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Win &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameBetInfo.CurrencyRatio &&
				compare.Bet == command.BetAmount &&
				compare.ValidBet == command.ValidBetAmount &&
				compare.Settle == command.SettleAmount &&
				compare.WinLoss == -command.CompanyWinLoss &&
				compare.Turnover == command.Turnover &&
				compare.Commission == command.Commission &&
				compare.Contribute == gameBetInfo.Contribute &&
				compare.Jackpot == gameBetInfo.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(1)
			.AdjustResettledBetManualAsync(
			walletId: Arg.Is(gameBetInfo.WalletId),
			amount: Arg.Is(command.AdjustAmount.Value),
			description: Arg.Is(command.Remark),
			externalId: Arg.Is(gameBetInfo.Id.ToString()),
			createdByUserId: Arg.Is(command.OperatorId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_betstate_is_waiting()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeGameBetsRapidTestWorkerClient = Substitute.For<IGameBetsRapidTestWorkerClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var roundId = Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552");
		var betAmount = 0.8m;
		var validBetAmount = 0.8m;
		var settleAmount = 1.2m;
		var commission = 0.0m;
		var turnover = 0.8m;
		var companyWinLoss = 0.4m;
		var clientStatus = Enums.RoundClientStatus.Win;
		var externalClosedOn = DateTimeOffset.Parse("2024-09-12T09:55:15+08:00");
		var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
		var remark = "測試Resettle調整";
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

		var gameBetInfo = new GameBets.Common.ViewModels.GameBetInfoViewModel(
			Id: Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552"),
			GameProviderId: Guid.Parse("147cef56-4fcd-4ee3-b748-87ad398f2871"),
			GameProviderConfigurationId: Guid.Parse("dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a"),
			GameId: 6700018,
			ResellerId: Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
			LicenseeId: Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
			BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4unykr72x8d9",
			CurrencyCode: "CNY",
			WalletId: Guid.Parse("8bff49a2-66ff-4c66-81f6-c9b242f2b868"),
			ExternalRoundId: "6323442723",
			ExternalBetId: "6323442723-01",
			ExternalCreatedOn: DateTimeOffset.Parse("2024-09-12T09:55:12+08:00"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Waiting,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 0.8m,
			ValidBet: 0.8m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UnixEpoch,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("{\"bet\": 0.8, \"win\": 1.2, \"rake\": 0, \"round\": \"6323442723\", \"detail\": [{\"freegame\": 0}, {\"luckydraw\": 0}, {\"bonus\": 0}], \"donate\": 0, \"status\": \"complete\", \"account\": \"4unykr72x8d9\", \"balance\": 84.4, \"bettime\": \"2024-09-11T21:55:12.737-04:00\", \"bettype\": [], \"jackpot\": 0, \"roomfee\": 0, \"tableid\": \"\", \"currency\": \"CNY\", \"gamecode\": \"20\", \"gamehall\": \"cq9\", \"gameplat\": \"web\", \"gamerole\": \"\", \"gametype\": \"slot\", \"isdonate\": false, \"validbet\": 0, \"tabletype\": \"\", \"bankertype\": \"\", \"createtime\": \"2024-09-11T21:55:15.456-04:00\", \"gameresult\": {\"cards\": [], \"points\": []}, \"jackpottype\": \"\", \"roundnumber\": \"\", \"endroundtime\": \"2024-09-11T21:55:15.555-04:00\", \"singlerowbet\": false, \"jackpotcontribution\": []}"),
			ExtendedData: new JsonObject { });
		_ = fakeGameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: Arg.Is(command.ExternalClosedOn.AddDays(-45)),
			id: Arg.Is(command.RoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameBetsClient,
			fakeGameBetsRapidTestWorkerClient,
			fakeCashServiceClient);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeGameBetsRapidTestWorkerClient
			.Received(0)
			.HandleAsync(
			source: Arg.Any<GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.AdjustResettledBetManualAsync(
			walletId: Arg.Any<Guid>(),
			amount: Arg.Any<decimal>(),
			description: Arg.Any<string>(),
			externalId: Arg.Any<string>(),
			createdByUserId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_betstate_is_running()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeGameBetsRapidTestWorkerClient = Substitute.For<IGameBetsRapidTestWorkerClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var roundId = Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552");
		var betAmount = 0.8m;
		var validBetAmount = 0.8m;
		var settleAmount = 1.2m;
		var commission = 0.0m;
		var turnover = 0.8m;
		var companyWinLoss = 0.4m;
		var clientStatus = Enums.RoundClientStatus.Win;
		var externalClosedOn = DateTimeOffset.Parse("2024-09-12T09:55:15+08:00");
		var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
		var remark = "測試Resettle調整";
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

		var gameBetInfo = new GameBets.Common.ViewModels.GameBetInfoViewModel(
			Id: Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552"),
			GameProviderId: Guid.Parse("147cef56-4fcd-4ee3-b748-87ad398f2871"),
			GameProviderConfigurationId: Guid.Parse("dec3dec2-ac0f-4f0d-a12e-eec0bc9e083a"),
			GameId: 6700018,
			ResellerId: Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
			LicenseeId: Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
			BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4unykr72x8d9",
			CurrencyCode: "CNY",
			WalletId: Guid.Parse("8bff49a2-66ff-4c66-81f6-c9b242f2b868"),
			ExternalRoundId: "6323442723",
			ExternalBetId: "6323442723-01",
			ExternalCreatedOn: DateTimeOffset.Parse("2024-09-12T09:55:12+08:00"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Running,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 0.8m,
			ValidBet: 0.8m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UnixEpoch,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("{\"bet\": 0.8, \"win\": 1.2, \"rake\": 0, \"round\": \"6323442723\", \"detail\": [{\"freegame\": 0}, {\"luckydraw\": 0}, {\"bonus\": 0}], \"donate\": 0, \"status\": \"complete\", \"account\": \"4unykr72x8d9\", \"balance\": 84.4, \"bettime\": \"2024-09-11T21:55:12.737-04:00\", \"bettype\": [], \"jackpot\": 0, \"roomfee\": 0, \"tableid\": \"\", \"currency\": \"CNY\", \"gamecode\": \"20\", \"gamehall\": \"cq9\", \"gameplat\": \"web\", \"gamerole\": \"\", \"gametype\": \"slot\", \"isdonate\": false, \"validbet\": 0, \"tabletype\": \"\", \"bankertype\": \"\", \"createtime\": \"2024-09-11T21:55:15.456-04:00\", \"gameresult\": {\"cards\": [], \"points\": []}, \"jackpottype\": \"\", \"roundnumber\": \"\", \"endroundtime\": \"2024-09-11T21:55:15.555-04:00\", \"singlerowbet\": false, \"jackpotcontribution\": []}"),
			ExtendedData: new JsonObject { });
		_ = fakeGameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: Arg.Is(command.ExternalClosedOn.AddDays(-45)),
			id: Arg.Is(command.RoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameBetsClient,
			fakeGameBetsRapidTestWorkerClient,
			fakeCashServiceClient);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeGameBetsRapidTestWorkerClient
			.Received(0)
			.HandleAsync(
			source: Arg.Any<GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.AdjustResettledBetManualAsync(
			walletId: Arg.Any<Guid>(),
			amount: Arg.Any<decimal>(),
			description: Arg.Any<string>(),
			externalId: Arg.Any<string>(),
			createdByUserId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_does_not_exist()
	{
		var faceLogger = NullLogger<GameRoundResettleCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeGameBetsRapidTestWorkerClient = Substitute.For<IGameBetsRapidTestWorkerClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var roundId = Guid.Parse("0191e3f1-e185-0000-0001-07e8778bb552");
		var betAmount = 0.8m;
		var validBetAmount = 0.8m;
		var settleAmount = 1.2m;
		var commission = 0.0m;
		var turnover = 0.8m;
		var companyWinLoss = 0.4m;
		var clientStatus = Enums.RoundClientStatus.Win;
		var externalClosedOn = DateTimeOffset.Parse("2024-09-12T09:55:15+08:00");
		var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
		var remark = "測試Resettle調整";
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

		_ = fakeGameBetsClient.GameBetMajorGetAsync(
			externalCreatedOn: Arg.Is(command.ExternalClosedOn.AddDays(-45)),
			id: Arg.Is(command.RoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<GameBets.Common.ViewModels.GameBetInfoViewModel>(null));

		var sut = new GameRoundResettleCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameBetsClient,
			fakeGameBetsRapidTestWorkerClient,
			fakeCashServiceClient);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeGameBetsRapidTestWorkerClient
			.Received(0)
			.HandleAsync(
			source: Arg.Any<GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.AdjustResettledBetManualAsync(
			walletId: Arg.Any<Guid>(),
			amount: Arg.Any<decimal>(),
			description: Arg.Any<string>(),
			externalId: Arg.Any<string>(),
			createdByUserId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
