using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class VerificationCacheRepositoryUnitTest
{
	[Fact]
	public async Task VerifyBackofficePlayerFundIn_Cache_Is_False()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var transferWallet = true;
		var currencyCode = "CNY";

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "backoffice", ":", "player", ":", "fundin", ":", brandCode, ":", gameProviderCode, ":", currencyCode);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(false);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyBackofficePlayerFundInAsync(brandId, brandCode, gameProviderId, gameProviderCode, transferWallet, currencyCode);

		_ = fakeVerification
			.Received(1)
			.VerifyBackofficePlayerFundInAsync(
			brandId: Arg.Is(brandId),
			brandCode: Arg.Is(brandCode),
			gameProviderId: Arg.Is(gameProviderId),
			gameProviderCode: Arg.Is(gameProviderCode),
			transferWallet: Arg.Is(transferWallet),
			currencyCode: Arg.Is(currencyCode),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(1)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyBackofficePlayerFundIn_Cache_Is_True()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var transferWallet = true;
		var currencyCode = "CNY";

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "backoffice", ":", "player", ":", "fundin", ":", brandCode, ":", gameProviderCode, ":", currencyCode);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(true);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyBackofficePlayerFundInAsync(brandId, brandCode, gameProviderId, gameProviderCode, transferWallet, currencyCode);

		_ = fakeVerification
			.Received(0)
			.VerifyBackofficePlayerFundInAsync(
			brandId: Arg.Is(brandId),
			brandCode: Arg.Is(brandCode),
			gameProviderId: Arg.Is(gameProviderId),
			gameProviderCode: Arg.Is(gameProviderCode),
			transferWallet: Arg.Is(transferWallet),
			currencyCode: Arg.Is(currencyCode),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(0)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyPlayerBalance_Cache_Is_False()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "balance", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(false);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyPlayerBalanceAsync(player, gameProvider);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerBalanceAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(1)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyPlayerBalance_Cache_Is_True()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "balance", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(true);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyPlayerBalanceAsync(player, gameProvider);

		_ = fakeVerification
			.Received(0)
			.VerifyPlayerBalanceAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(0)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyPlayerFund_Cache_Is_False()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "fund", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(false);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyPlayerFundAsync(player, gameProvider);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerFundAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(1)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyPlayerFund_Cache_Is_True()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "fund", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(true);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyPlayerFundAsync(player, gameProvider);

		_ = fakeVerification
			.Received(0)
			.VerifyPlayerFundAsync(
			player: Arg.Is(player),
			gameProvider: Arg.Is(gameProvider),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(0)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyPlayerGameUrl_Cache_Is_False()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
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
			AvailableCurrencies: null,
			SubGameProviderCode: "KY");

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "gameurl", ":", player.PlayerCode, ":", game.GameProviderCode, ":", game.GameId);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(false);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyPlayerGameUrlAsync(player, game);

		_ = fakeVerification
			.Received(1)
			.VerifyPlayerGameUrlAsync(
			player: Arg.Is(player),
			game: Arg.Is(game),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(1)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}

	[Fact]
	public async Task VerifyPlayerGameUrl_Cache_Is_True()
	{
		var fakeVerification = Substitute.For<IVerification>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpiration = TimeSpan.FromMinutes(10);

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
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
			AvailableCurrencies: null,
			SubGameProviderCode: "KY");

		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "gameurl", ":", player.PlayerCode, ":", game.GameProviderCode, ":", game.GameId);

		_ = fakeMemoryCache.TryGetValue(
			key: Arg.Is(key),
			value: out _)
			.Returns(true);

		var sut = new VerificationCacheRepository(
			fakeVerification,
			fakeMemoryCache,
			absoluteExpiration);

		await sut.VerifyPlayerGameUrlAsync(player, game);

		_ = fakeVerification
			.Received(0)
			.VerifyPlayerGameUrlAsync(
			player: Arg.Is(player),
			game: Arg.Is(game),
			Arg.Any<CancellationToken>());

		_ = fakeMemoryCache
			.Received(0)
			.Set(
			key: key,
			value: DateTime.UtcNow);
	}
}
