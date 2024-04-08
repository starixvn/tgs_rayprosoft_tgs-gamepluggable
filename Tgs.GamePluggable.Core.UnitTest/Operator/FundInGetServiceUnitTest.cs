using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core.Models;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class FundInGetServiceUnitTest
{
	[Fact]
	public async Task Fund_In()
	{
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

		var command = new FundInCommand(
			"4ucmqplxb029",
			"ky",
			1);

		var player = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayer
			.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(player));

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

		var gameProvider = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.Kaiyuan,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProvider
			.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProvider));

		var platformBalance = new PlatformBalanceInfo(
			Balance: 100.0m,
			FreeBalance: 10.0m,
			OutstandingBalance: 90.0m);
		_ = fakePlayerGameBalance
			.GetBalanceAsync(
			gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
			playerCode: Arg.Is(player.PlayerCode),
			currencyCode: Arg.Is(player.CurrencyCode))
			.Returns(platformBalance);

		var fakeService = new FundInCommandService(
			fakeGameFundTransferClient,
			fakeGameProvider,
			fakeVerification,
			fakePlayer,
			fakePlayerGameBalance);

		await fakeService.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerFundAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundInAsync(
			gameProviderId: Arg.Is(gameProviderId),
			gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
			brandCode: Arg.Is(player.BrandCode),
			playerCode: Arg.Is(player.PlayerCode),
			walletId: Arg.Is(player.WalletId),
			currencyCode: Arg.Is(player.CurrencyCode),
			amount: Arg.Is(command.Amount),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GameProvider_SM01_Fund_In_Amount_Only_Integer()
	{
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

		var command = new FundInCommand(
			"4ucmqplxb029",
			"sm01",
			123M);

		var player = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayer
			.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(player));

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

		var gameProvider = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.SM01,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProvider
			.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProvider));

		var platformBalance = new PlatformBalanceInfo(
			Balance: 1000.0m,
			FreeBalance: 123.456m,
			OutstandingBalance: 876.544m);
		_ = fakePlayerGameBalance
			.GetBalanceAsync(
			gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
			playerCode: Arg.Is(player.PlayerCode),
			currencyCode: Arg.Is(player.CurrencyCode))
			.Returns(platformBalance);

		var fakeService = new FundInCommandService(
			fakeGameFundTransferClient,
			fakeGameProvider,
			fakeVerification,
			fakePlayer,
			fakePlayerGameBalance);

		await fakeService.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerFundAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundInAsync(
			gameProviderId: Arg.Is(gameProviderId),
			gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
			brandCode: Arg.Is(player.BrandCode),
			playerCode: Arg.Is(player.PlayerCode),
			walletId: Arg.Is(player.WalletId),
			currencyCode: Arg.Is(player.CurrencyCode),
			amount: Arg.Is(123M),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
