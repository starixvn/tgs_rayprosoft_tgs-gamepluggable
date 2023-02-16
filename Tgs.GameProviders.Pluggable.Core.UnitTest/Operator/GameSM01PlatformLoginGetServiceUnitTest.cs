using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

[TestFixture]
public class GameSM01PlatformLoginGetServiceUnitTest
{
    [Test]
    public async Task Get_SM01_Platform_Login()
    {
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameCommandClient = Substitute.For<IGameCommandClient>();

        var gameId = 5200001;
        var gameProviderId = Guid.Parse("ba2997d9-b856-4589-91ae-5060a952cc3c");

        var query = new GameSM01PlatformLoginQuery(
             "4ucmqplxb029");

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

        _ = fakeGameProvider
            .GetGameProviderIdAsync(GameProviderType.SM01)
            .Returns(gameProviderId);

        var games = new GameInfo[]
        {
            new GameInfo(
                GameId: gameId,
                GameCode: "sm01",
                GameProviderId: gameProviderId,
                GameProviderCode: GameProviderType.SM01,
                GameExternalId: "sm01",
                Type: GameType.Sportsbook,
                PlatformType: GamePlatformType.DesktopAndMobile,
                Name: GameProviderType.SM01,
                SubGame: false,
                AvailableToBrand: false,
                HasTurnoverOnTieBet: false,
                Html5: true,
                Description:  GameProviderType.SM01,
                EndpointPath: string.Empty,
                RuleUrl: string.Empty,
                AvailableCurrencies: null,
                Localization: null,
                SubGameProviderCode:  GameProviderType.SM01)
        };

        _ = fakeGameCommandClient
            .GameQueryAsync(gameProviderId)
            .Returns(games.ToAsyncEnumerable());

        _ = fakeGameVerificationClient
            .VerifyPlayerGameUrlAsync(
                Arg.Is(query.PlayerCode),
                Arg.Is(gameId))
            .Returns(true);

        var loginInfo = new GameSM01PlatformLoginInfo(
            "https://cn.ucfclive.com",
            "SQjCxm2cX5bahBta",
            "4ucmqplxb029",
            1670390195123,
            "46C9EA4F03900B74379A15A56ED14229");

        _ = fakeGameUrl
            .GetGamePlatformLoginBySM01Async(
                Arg.Is(player.PlayerCode))
            .Returns(ValueTask.FromResult(loginInfo));

        var sut = new GameSM01PlatformLoginGetService(
            fakeGameUrl,
            fakeGameVerificationClient,
            fakeGameProvider,
            fakeGameCommandClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(loginInfo.MerchantId, actual.MerchantId);
        Assert.AreEqual(loginInfo.CustomerName, actual.CustomerName);
        Assert.AreEqual(loginInfo.Timestamp, actual.Timestamp);
        Assert.AreEqual(loginInfo.Sign, actual.Sign);
    }
}
