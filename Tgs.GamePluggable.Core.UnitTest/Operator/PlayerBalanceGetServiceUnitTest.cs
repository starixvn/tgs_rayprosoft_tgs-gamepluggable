using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core.Models;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Player_Balance()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

		var query = new PlayerBalanceQuery(
			"4ucmqplxb029",
			"KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(query.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: query.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundTransferBalanceResult = new GameFundTransfer.Common.ViewModels.FundTransferBalanceResultViewModel(
			Balance: 1000.00m,
			FreeBalance: 100.00m,
			OutstandingBalance: 900.00m);
		_ = fakeGameFundTransferClient.FundTransferBalanceAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(fundTransferBalanceResult);

		var fakeService = new PlayerBalanceGetService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		var actual = await fakeService.GetAsync(query);

		Assert.Equal(fundTransferBalanceResult.Balance, actual.Balance);
		Assert.Equal(fundTransferBalanceResult.FreeBalance, actual.FreeBalance);
		Assert.Equal(fundTransferBalanceResult.OutstandingBalance, actual.OutstandingBalance);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerBalanceAsync(
			player: Arg.Is(playerInfo),
			gameProvider: Arg.Is(gameProviderInfo),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
