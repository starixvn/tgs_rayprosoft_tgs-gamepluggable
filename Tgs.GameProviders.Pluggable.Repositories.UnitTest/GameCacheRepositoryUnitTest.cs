using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class GameCacheRepositoryUnitTest
{
    [Fact]
    public async Task Get()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

        var gameId = 1;

        var key = $"game:pluggable:gamecache:get:{gameId}";

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);
        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.Kaiyuan,
            GameExternalId: "lobby",
            Type: GameType.CardGameLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.Kaiyuan,
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: GameProviderType.Kaiyuan,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.Kaiyuan);
        _ = fakeGame
            .GetAsync(
                gameId: Arg.Is(gameId))
            .Returns(game);

        var repository = new GameCacheRepository(
            fakeGame,
            fakeMemoryCache,
            absoluteExpirationRelativeToNow);

        _ = await repository
           .GetAsync(
               gameId)
           .ConfigureAwait(false);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                game);
    }
}
