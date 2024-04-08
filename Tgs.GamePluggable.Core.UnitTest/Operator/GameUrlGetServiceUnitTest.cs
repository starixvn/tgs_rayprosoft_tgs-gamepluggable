using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Core.UnitTest.Operator;

public class GameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Game_Url()
	{
		var fakeGame = Substitute.For<IGameRepository>();
		var fakeGameUrl = Substitute.For<IGameUrl>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new GameUrlQuery(
			 PlayerCode: "4ucmqplxb029",
			 LoginRecordId: Guid.NewGuid(),
			 PlatformType: GameProvidersClientPlatformType.Desktop,
			 GameId: 1000001,
			 Language: "zh-CN",
			 IpAddress: "127.0.0.1",
			 BackUrl: new("https://dev-t1.rpgp.cc/"));

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

		var game = new Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.Kaiyuan,
			ExternalGameId: "lobby",
			Type: Enums.GameType.CardGameLobby,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);
		_ = fakeGame
			.GetAsync(
			id: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(game));

		var gameUrl = new Uri("https://www.google.com/");
		_ = fakeGameUrl
			.GetGameUrlAsync(
			gameProviderCode: Arg.Is(game.GameProviderCode),
			gameType: Arg.Is((GameProvidersClientGameType)game.Type),
			gameId: Arg.Is(game.GameId),
			brandCode: Arg.Is(player.BrandCode),
			playerCode: Arg.Is(player.PlayerCode),
			currencyCode: Arg.Is(player.CurrencyCode),
			loginRecordId: Arg.Is(query.LoginRecordId),
			platformType: Arg.Is(query.PlatformType),
			language: Arg.Is(query.Language),
			ipAddress: Arg.Is(query.IpAddress),
			backUrl: Arg.Is(query.BackUrl))
			.Returns(ValueTask.FromResult(gameUrl));

		var sut = new GameUrlGetService(
			fakeGame,
			fakeGameUrl,
			fakeVerification,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerGameUrlAsync(
			player: Arg.Is(player),
			game: Arg.Is(game),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Get_AsiaGaming_Game_Url()
	{
		var fakeGame = Substitute.For<IGameRepository>();
		var fakeGameUrl = Substitute.For<IGameUrl>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new GameUrlQuery(
			 PlayerCode: "4ucmqplxb029",
			 LoginRecordId: Guid.NewGuid(),
			 PlatformType: GameProvidersClientPlatformType.Desktop,
			 GameId: 1000001,
			 Language: "zh-CN",
			 IpAddress: "127.0.0.1",
			 BackUrl: new("https://dev-t1.rpgp.cc/"));

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

		var game = new Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.AsiaGaming,
			ExternalGameId: "lobby",
			Type: Enums.GameType.LotteryLobby,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);
		_ = fakeGame
			.GetAsync(
			id: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(game));

		var gameUrl = new Uri("https://www.google.com/");
		_ = fakeGameUrl
			.GetGameUrlByAsiaGamingAsync(
			gameProviderCode: Arg.Is(game.GameProviderCode),
			gameType: Arg.Is((GameProvidersClientGameType)game.Type),
			gameId: Arg.Is(game.GameId),
			brandCode: Arg.Is(player.BrandCode),
			playerCode: Arg.Is(player.PlayerCode),
			currencyCode: Arg.Is(player.CurrencyCode),
			loginRecordId: Arg.Is(query.LoginRecordId),
			platformType: Arg.Is(query.PlatformType),
			language: Arg.Is(query.Language),
			ipAddress: Arg.Is(query.IpAddress),
			backUrl: Arg.Is(query.BackUrl),
			defaultWalletId: Arg.Is(player.WalletId))
			.Returns(ValueTask.FromResult(gameUrl));

		var sut = new GameUrlGetService(
			fakeGame,
			fakeGameUrl,
			fakeVerification,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerGameUrlAsync(
			   player: Arg.Is(player),
			   game: Arg.Is(game),
			   cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Get_BetConstruct_Game_Url()
	{
		var fakeGame = Substitute.For<IGameRepository>();
		var fakeGameUrl = Substitute.For<IGameUrl>();
		var fakeVerification = Substitute.For<IVerification>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new GameUrlQuery(
			 "4ucmqplxb029",
			 Guid.NewGuid(),
			 GameProvidersClientPlatformType.Desktop,
			 1000001,
			 "zh-CN",
			 "127.0.0.1",
			 new("https://dev-t1.rpgp.cc/"));

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

		var game = new Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.BetConstruct,
			ExternalGameId: "lobby",
			Type: Enums.GameType.Sportsbook,
			PlatformType: Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);
		_ = fakeGame
			.GetAsync(
			id: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(game));

		var gameUrl = new Uri("https://www.google.com/");
		_ = fakeGameUrl
			.GetGameUrlByBetConstructAsync(
			gameProviderCode: Arg.Is(game.GameProviderCode),
			gameType: Arg.Is((GameProvidersClientGameType)game.Type),
			gameId: Arg.Is(game.GameId),
			brandCode: Arg.Is(player.BrandCode),
			playerCode: Arg.Is(player.PlayerCode),
			currencyCode: Arg.Is(player.CurrencyCode),
			loginRecordId: Arg.Is(query.LoginRecordId),
			platformType: Arg.Is(query.PlatformType),
			language: Arg.Is(query.Language),
			ipAddress: Arg.Is(query.IpAddress),
			backUrl: Arg.Is(query.BackUrl),
			brandId: Arg.Is(player.BrandId),
			gameProviderId: Arg.Is(game.GameProviderId))
			.Returns(ValueTask.FromResult(gameUrl));

		var sut = new GameUrlGetService(
			fakeGame,
			fakeGameUrl,
			fakeVerification,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerGameUrlAsync(
			player: Arg.Is(player),
			game: Arg.Is(game),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
