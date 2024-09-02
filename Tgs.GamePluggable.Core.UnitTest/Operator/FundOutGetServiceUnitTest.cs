using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core.Models;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class FundOutGetServiceUnitTest
{
	[Fact]
	public async Task Fund_Out()
	{
		var faceLogger = NullLogger<FundInCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundOutCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: 1.00m);

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var balance = 10.0m;
		_ = fakeCashServiceClient.GetBalanceAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(balance));

		var fakeService = new FundOutCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeCashServiceClient,
			fakeGameFundTransferClient);

		await fakeService.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerFundAsync(
			player: Arg.Is(playerInfo),
			gameProvider: Arg.Is(gameProviderInfo),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundTransferOutAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			amount: Arg.Is(1.00m),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Theory]
	[InlineData(0.99)]
	[InlineData(10000001)]
	public async Task The_field_Amount_must_be_between_1_and_10000000(decimal amount)
	{
		var faceLogger = NullLogger<FundInCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundOutCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: amount);

		var fakeService = new FundOutCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeCashServiceClient,
			fakeGameFundTransferClient);

		await fakeService.ExecuteAsync(command);

		_ = fakeVerification
			.Received(0)
			.VerifyPlayerFundAsync(
			player: Arg.Any<Models.PlayerInfo>(),
			gameProvider: Arg.Any<Models.GameProviderInfo>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferOutAsync(
			playerCode: Arg.Any<string>(),
			gameProviderCode: Arg.Any<string>(),
			amount: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Timeout_Exception()
	{
		var faceLogger = NullLogger<FundInCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundOutCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: 1.00m);

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		_ = fakeCashServiceClient
			.GetBalanceAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			cancellationToken: Arg.Any<CancellationToken>())
			.ThrowsAsync(new TimeoutException());

		var fakeService = new FundOutCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeCashServiceClient,
			fakeGameFundTransferClient);

		_ = await Assert.ThrowsAsync<TimeoutException>(() => fakeService.ExecuteAsync(command).AsTask());

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerFundAsync(
			player: Arg.Is(playerInfo),
			gameProvider: Arg.Is(gameProviderInfo),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferOutAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			amount: Arg.Is(1.00m),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ArgumentOutOfRangeException_ThrowIfLessThan()
	{
		var faceLogger = NullLogger<FundInCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundOutCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: 11.00m);

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var balance = 10.0m;
		_ = fakeCashServiceClient.GetBalanceAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(balance));

		var fakeService = new FundOutCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeCashServiceClient,
			fakeGameFundTransferClient);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => fakeService.ExecuteAsync(command).AsTask());

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerFundAsync(
			player: Arg.Is(playerInfo),
			gameProvider: Arg.Is(gameProviderInfo),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferOutAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			amount: Arg.Is(1.00m),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
