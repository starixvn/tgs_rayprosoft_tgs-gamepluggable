using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class GameProviderRepositoryUnitTest
{
    [Fact]
    public async Task Get()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.Kaiyuan,
            UniqueId: 1,
            Name: GameProviderType.Kaiyuan,
            TransferWallet: true,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.Kaiyuan,
            Localization: null);
        _ = fakeClient
            .GameProviderGetAsync(
                gameProviderId: Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var repository = new GameProviderRepository(
            fakeClient);

        var actual = await repository.GetAsync(gameProviderId);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetConfiguration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

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
        _ = fakeClient
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: Arg.Is(gameProviderConfigId))
            .Returns(gameProviderConfiguration);

        var repository = new GameProviderRepository(
            fakeClient);

        var actual = await repository.GetConfigurationAsync(gameProviderConfigId);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetGameProviderId()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderCode = "KY";

        var gameProviderId = Guid.NewGuid();
        _ = fakeClient
            .GameProviderGetGameProviderIdAsync(
                gameProviderCode: Arg.Is(gameProviderCode))
            .Returns(gameProviderId);

        var repository = new GameProviderRepository(
            fakeClient);

        var actual = await repository.GetGameProviderIdAsync(gameProviderCode);

        Assert.Equal(gameProviderId, actual);
    }
}
