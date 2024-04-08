using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Player_Balance()
	{
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

		var query = new PlayerBalanceQuery(
			"4ucmqplxb029",
			"KY");

		var player = new Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayer
			.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(player));

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			code: Arg.Is(query.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

		var gameProvider = new Models.GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.Kaiyuan,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProvider
			.GetAsync(
			id: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProvider));

		var platformBalance = new Models.PlatformBalanceInfo(
			Balance: 10000m,
			FreeBalance: 1000m,
			OutstandingBalance: 9000m);
		_ = fakePlayerGameBalance
			.GetBalanceAsync(
			gameProviderCode: Arg.Is(query.GameProviderCode),
			playerCode: Arg.Is(player.PlayerCode),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(platformBalance);

		var fakeService = new PlayerBalanceGetService(
			fakePlayer,
			fakeGameProvider,
			fakeVerification,
			fakePlayerGameBalance);

		var actual = await fakeService.GetAsync(query);

		Assert.Equal(platformBalance.Balance, actual.Balance);
		Assert.Equal(platformBalance.FreeBalance, actual.FreeBalance);
		Assert.Equal(platformBalance.OutstandingBalance, actual.OutstandingBalance);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerBalanceAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
