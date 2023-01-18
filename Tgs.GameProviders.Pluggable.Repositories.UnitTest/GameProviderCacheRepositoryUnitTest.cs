using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

[TestClass]
public class GameProviderCacheRepositoryUnitTest
{
    [TestMethod]
    public async Task Get()
    {
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = Guid.NewGuid();

        var key = $"game:pluggable:gameprovidercache:get:{gameProviderId}";

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.KY,
            UniqueId: 1,
            Name: GameProviderType.KY,
            TransferWallet: true,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.KY,
            Localization: null);
        _ = fakeGameProvider
            .GetAsync(
                gameProviderId: Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var repository = new GameProviderCacheRepository(
            fakeGameProvider,
            fakeMemoryCache);

        var actual = await repository
           .GetAsync(
               gameProviderId)
           .ConfigureAwait(false);

        Assert.IsNotNull(actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                gameProvider);
    }

    [TestMethod]
    public async Task GetConfiguration()
    {
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderConfigId = Guid.NewGuid();

        var gameProviderConfiguration = new GameProviderConfigInfo(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.BetConstruct,
            GameProviderConfigId: gameProviderConfigId,
            Status: GameProviderConfigStatus.Active,
            Name: $"{GameProviderType.BetConstruct} Configuration",
            Description: $"{GameProviderType.BetConstruct} Configuration",
            ClientId: $"{GameProviderType.BetConstruct}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            DesktopEndpoint: "https://devsportsbook.srvttt.com",
            MobileEndpoint: "https://devmobile.srvttt.com",
            ConfigurationData: string.Empty,
            IsDefault: false);
        _ = fakeGameProvider
            .GetConfigurationAsync(
                gameProviderConfigId: Arg.Is(gameProviderConfigId))
            .Returns(gameProviderConfiguration);

        var repository = new GameProviderCacheRepository(
            fakeGameProvider,
            fakeMemoryCache);

        var actual = await repository
           .GetConfigurationAsync(
               gameProviderConfigId)
           .ConfigureAwait(false);

        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public async Task GetGameProviderId()
    {
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderCode = "KY";

        var key = $"game:pluggable:gameprovidercache:get:gameproviderid:{gameProviderCode}";

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(gameProviderCode))
            .Returns(gameProviderId);

        var repository = new GameProviderCacheRepository(
            fakeGameProvider,
            fakeMemoryCache);

        var actual = await repository
           .GetGameProviderIdAsync(
               gameProviderCode)
           .ConfigureAwait(false);

        Assert.IsNotNull(actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                gameProviderId);
    }
}
