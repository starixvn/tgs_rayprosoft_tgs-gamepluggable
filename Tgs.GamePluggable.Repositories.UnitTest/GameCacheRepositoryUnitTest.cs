using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GamePluggable.Core;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class GameCacheRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var fakeGame = Substitute.For<IGameRepository>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

		var gameId = 1;

		var key = string.Concat(typeof(GameCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "game", ":", "get", ":", gameId);

		_ = fakeMemoryCache
			.TryGetValue(
				key: Arg.Is(key),
				value: out _)
			 .Returns(false);
		var game = new Core.Models.GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.Kaiyuan,
			ExternalGameId: "lobby",
			Type: Core.Enums.GameType.CardGameLobby,
			PlatformType: Core.Enums.GamePlatformType.DesktopAndMobile,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null,
			SubGameProviderCode: GameProviderType.Kaiyuan);
		_ = fakeGame
			.GetAsync(
				id: Arg.Is(gameId))
			.Returns(game);

		var repository = new GameCacheRepository(
			fakeGame,
			fakeMemoryCache,
			absoluteExpirationRelativeToNow);

		_ = await repository.GetAsync(gameId);

		_ = fakeMemoryCache
			.Received(1)
			.Set(
				key: key,
				game);
	}
}
