using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class FundInGetServiceUnitTest
{
	[Fact]
	public async Task Fund_In()
	{
		var faceLogger = NullLogger<FundInCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundInCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: 1.00m);

		var playerInfo = new Models.PlayerInfo(
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

		var gameProviderInfo = new Models.GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 100.00m,
			FreeBalance = 10.00m,
			OutstandingBalance = 90.00m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var fakeService = new FundInCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
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
			.FundInResultAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			walletId: Arg.Is(playerInfo.WalletId),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
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
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundInCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: amount);

		var fakeService = new FundInCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
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
			.FundInResultAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			walletId: Arg.Any<Guid>(),
			currencyCode: Arg.Any<string>(),
			amount: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ArgumentOutOfRangeException_ThrowIfLessThan()
	{
		var faceLogger = NullLogger<FundInCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var command = new FundInCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderCode: "ky",
			Amount: 2.00m);

		var playerInfo = new Models.PlayerInfo(
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

		var gameProviderInfo = new Models.GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1.00m,
			FreeBalance = 1.00m,
			OutstandingBalance = 0.00m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var fakeService = new FundInCommandService(
			faceLogger,
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
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
			.FundInResultAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			walletId: Arg.Any<Guid>(),
			currencyCode: Arg.Any<string>(),
			amount: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
