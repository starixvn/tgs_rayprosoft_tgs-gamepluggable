using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GamePluggable.Core.Models;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class GameSM01PlatformLoginGetServiceUnitTest
{
	[Fact]
	public async Task Get_SM01_Platform_Login()
	{
		var fakeGameUrl = Substitute.For<IGameUrl>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();

		var gameId = 5200001;
		var gameProviderId = Guid.Parse("ba2997d9-b856-4589-91ae-5060a952cc3c");

		var query = new GameSM01PlatformLoginQuery(
			 PlayerCode: "4ucmqplxb029");

		var player = new PlayerInfo(
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

		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			Arg.Is(GameProviderType.SM01),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		_ = fakeGameRepository.QueryAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { gameId }.ToAsyncEnumerable());

		var game = new GameInfo(
			GameId: gameId,
			GameCode: "sm01",
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.SM01,
			ExternalGameId: "sm01",
			Type: Enums.GameType.Sportsbook,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: string.Empty,
			RuleUrl: string.Empty,
			AvailableCurrencies: null);

		_ = fakeGameRepository.GetAsync(
			id: Arg.Is(gameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var loginInfo = new GameSM01PlatformLoginInfo(
			ApiUrl: "https://cn.ucfclive.com",
			MerchantId: "SQjCxm2cX5bahBta",
			CustomerName: "4ucmqplxb029",
			Timestamp: 1670390195123,
			Sign: "46C9EA4F03900B74379A15A56ED14229");

		_ = fakeGameUrl
			.GetGamePlatformLoginBySM01Async(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(loginInfo));

		var sut = new GameSM01PlatformLoginGetService(
			fakeGameUrl,
			fakeVerification,
			fakeGameProvider,
			fakePlayer,
			fakeGameRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(loginInfo.MerchantId, actual.MerchantId);
		Assert.Equal(loginInfo.CustomerName, actual.CustomerName);
		Assert.Equal(loginInfo.Timestamp, actual.Timestamp);
		Assert.Equal(loginInfo.Sign, actual.Sign);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerGameUrlAsync(
			player: Arg.Is(player),
			game: Arg.Is(game),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
