using NSubstitute;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Exceptions;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class VerifyPlayerGameUrlUnitTest
{
	[Fact]
	public async Task Brand_Currency_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
			() => sut.VerifyPlayerGameUrlAsync(
				player: player,
				game: game)
			.AsTask());
	}

	[Fact]
	public async Task Brand_Game_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
			() => sut.VerifyPlayerGameUrlAsync(
				player: player,
				game: game)
			.AsTask());
	}

	[Fact]
	public async Task GameProvider_Currency_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
			() => sut.VerifyPlayerGameUrlAsync(
				player: player,
				game: game)
			.AsTask());
	}

	[Fact]
	public async Task Player_State_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
			() => sut.VerifyPlayerGameUrlAsync(
				player: player,
				game: game)
			.AsTask());
	}

	[Fact]
	public async Task Game_Available_Currency_Is_Not_CNY()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: ["PHP", "HKD"]);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
			() => sut.VerifyPlayerGameUrlAsync(
				player: player,
				game: game)
			.AsTask());
	}

	[Fact]
	public async Task Game_Available_Currency_Is_CNY()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: ["PHP", "HKD", "CNY"]);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		await sut.VerifyPlayerGameUrlAsync(
			player: player,
			game: game);
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameId: Arg.Is(game.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(game.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		await sut.VerifyPlayerGameUrlAsync(
			player: player,
			game: game);
	}
}
