using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

[TestClass]
public class GameUrlRepositoryUnitTest
{
    [TestMethod]
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
            .Returns(ValueTask.FromResult<Uri?>(gameUrl));

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

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }

    [TestMethod]
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
            .Returns(ValueTask.FromResult<Uri?>(gameUrl));

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

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }

    [TestMethod]
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
            gameProviderConfigurationId,
            gameProviderId,
            "https://devsportsbook.srvttt.com",
            "https://devmobile.srvttt.com",
            string.Empty);
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
            .Returns(ValueTask.FromResult<Uri?>(gameUrl));

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

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }

    [TestMethod]
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
            gameProviderConfigurationId,
            gameProviderId,
            "https://devsportsbook.srvttt.com",
            "https://devmobile.srvttt.com",
            string.Empty);
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
            .Returns(ValueTask.FromResult<Uri?>(gameUrl));

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

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }
}
