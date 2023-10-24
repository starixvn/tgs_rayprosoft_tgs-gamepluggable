using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class PlayerCacheRepositoryUnitTest
{
    [Fact]
    public async Task Get()
    {
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

        var playerCode = "4ucmqplxb029";

        var key = $"game:pluggable:playercache:get:{playerCode}";

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var player = new PlayerInfo(
            Guid.NewGuid(),
            "TGS",
            Guid.NewGuid(),
            "TGS",
            Guid.NewGuid(),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.NewGuid(),
            "CNY");
        _ = fakePlayer
            .GetAsync(
                playerCode: Arg.Is(playerCode))
            .Returns(player);

        var repository = new PlayerCacheRepository(
            fakePlayer,
            fakeMemoryCache,
            absoluteExpirationRelativeToNow);

        _ = await repository.GetAsync(playerCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                player);
    }
}
