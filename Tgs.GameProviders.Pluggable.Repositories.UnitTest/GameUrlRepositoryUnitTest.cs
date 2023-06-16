using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class GameUrlRepositoryUnitTest
{
    [Fact]
    public async Task Normal_Process_Game_Url()
    {
        var fakeBrand = Substitute.For<IBrand>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var gameProviderCode = "KY";
        var gameType = GameProvidersClientGameType.CardGameLobby;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "Test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.01";
        var backUrl = new Uri("https://dev-t1.rpgp.cc");

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameProvidersClient
            .GameUrlAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(gameType),
                Arg.Is(gameId),
                Arg.Is(brandCode),
                Arg.Is(playerCode),
                Arg.Is(currencyCode),
                Arg.Is(loginRecordId),
                Arg.Is(platformType),
                Arg.Is(language),
                Arg.Is(ipAddress),
                Arg.Is(backUrl))
            .Returns(gameUrl);

        var sut = new GameUrlRepository(
            fakeBrand,
            fakeCashServiceClient,
            fakeGameProvider,
            fakeGameProvidersClient);

        var actual = await sut
             .GetGameUrlAsync(
                 gameProviderCode,
                 gameType,
                 gameId,
                 brandCode,
                 playerCode,
                 currencyCode,
                 loginRecordId,
                 platformType,
                 language,
                 ipAddress,
                 backUrl)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);
    }

    [Fact]
    public async Task Normal_Process_Get_GameUrl_By_AsiaGaming()
    {
        var fakeBrand = Substitute.For<IBrand>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var gameProviderCode = "AG";
        var gameType = GameProvidersClientGameType.LiveDealerLobby;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "Test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.01";
        var backUrl = new Uri("https://dev-t1.rpgp.cc");
        var defaultWalletId = Guid.NewGuid();

        var gameBalance = 100m;
        _ = fakeCashServiceClient
            .GetBalanceAsync(
                Arg.Is(defaultWalletId))
            .Returns(gameBalance);

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameProvidersClient
            .GameUrlByAsiaGamingAsync(
                Arg.Is(gameType),
                Arg.Is(gameId),
                Arg.Is(brandCode),
                Arg.Is(playerCode),
                Arg.Is(currencyCode),
                Arg.Is(loginRecordId),
                Arg.Is(platformType),
                Arg.Is(language),
                Arg.Is(ipAddress),
                Arg.Is(backUrl),
                Arg.Is(gameBalance))
            .Returns(gameUrl);

        var sut = new GameUrlRepository(
            fakeBrand,
            fakeCashServiceClient,
            fakeGameProvider,
            fakeGameProvidersClient);

        var actual = await sut
             .GetGameUrlByAsiaGamingAsync(
                 gameProviderCode,
                 gameType,
                 gameId,
                 brandCode,
                 playerCode,
                 currencyCode,
                 loginRecordId,
                 platformType,
                 language,
                 ipAddress,
                 backUrl,
                 defaultWalletId)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);
    }

    [Fact]
    public async Task Normal_Process_Get_GameUrl_By_BetConstruct_Desktop()
    {
        var fakeBrand = Substitute.For<IBrand>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var gameProviderCode = "BC";
        var gameType = GameProvidersClientGameType.Sportsbook;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "Test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.01";
        var backUrl = new Uri("https://dev-t1.rpgp.cc");
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetGameProviderConfigurationIdAsync(
                Arg.Is(brandId),
                Arg.Is(gameProviderId))
            .Returns(gameProviderConfigurationId);

        var gameProviderConfiguration = new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.BetConstruct,
            GameProviderConfigId: gameProviderConfigurationId,
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
                Arg.Is(gameProviderConfigurationId))
            .Returns(gameProviderConfiguration);

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameProvidersClient
            .GameUrlByBetConstructAsync(
                Arg.Is(gameType),
                Arg.Is(gameId),
                Arg.Is(brandCode),
                Arg.Is(playerCode),
                Arg.Is(currencyCode),
                Arg.Is(loginRecordId),
                Arg.Is(platformType),
                Arg.Is(language),
                Arg.Is(ipAddress),
                Arg.Is(backUrl),
                Arg.Is(gameProviderConfiguration.DesktopEndpoint))
            .Returns(gameUrl);

        var sut = new GameUrlRepository(
            fakeBrand,
            fakeCashServiceClient,
            fakeGameProvider,
            fakeGameProvidersClient);

        var actual = await sut
             .GetGameUrlByBetConstructAsync(
                 gameProviderCode,
                 gameType,
                 gameId,
                 brandCode,
                 playerCode,
                 currencyCode,
                 loginRecordId,
                 platformType,
                 language,
                 ipAddress,
                 backUrl,
                 brandId,
                 gameProviderId)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);
    }

    [Fact]
    public async Task Normal_Process_Get_GameUrl_By_BetConstruct_Mobile()
    {
        var fakeBrand = Substitute.For<IBrand>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

        var gameProviderCode = "BC";
        var gameType = GameProvidersClientGameType.Sportsbook;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "Test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Mobile;
        var language = "zh-TW";
        var ipAddress = "127.0.01";
        var backUrl = new Uri("https://dev-t1.rpgp.cc");
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetGameProviderConfigurationIdAsync(
                Arg.Is(brandId),
                Arg.Is(gameProviderId))
            .Returns(gameProviderConfigurationId);

        var gameProviderConfiguration = new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.BetConstruct,
            GameProviderConfigId: gameProviderConfigurationId,
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
                Arg.Is(gameProviderConfigurationId))
            .Returns(gameProviderConfiguration);

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameProvidersClient
            .GameUrlByBetConstructAsync(
                Arg.Is(gameType),
                Arg.Is(gameId),
                Arg.Is(brandCode),
                Arg.Is(playerCode),
                Arg.Is(currencyCode),
                Arg.Is(loginRecordId),
                Arg.Is(platformType),
                Arg.Is(language),
                Arg.Is(ipAddress),
                Arg.Is(backUrl),
                Arg.Is(gameProviderConfiguration.MobileEndpoint))
            .Returns(gameUrl);

        var sut = new GameUrlRepository(
            fakeBrand,
            fakeCashServiceClient,
            fakeGameProvider,
            fakeGameProvidersClient);

        var actual = await sut
             .GetGameUrlByBetConstructAsync(
                 gameProviderCode,
                 gameType,
                 gameId,
                 brandCode,
                 playerCode,
                 currencyCode,
                 loginRecordId,
                 platformType,
                 language,
                 ipAddress,
                 backUrl,
                 brandId,
                 gameProviderId)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);
    }
}
