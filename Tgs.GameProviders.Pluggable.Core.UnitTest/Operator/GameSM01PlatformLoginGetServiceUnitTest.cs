using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;
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
        var fakeGameProvidersQueryServerClient = Substitute.For<IGameProvidersQueryServerClient>();

        var gameId = 5200001;
        var gameProviderId = Guid.Parse("ba2997d9-b856-4589-91ae-5060a952cc3c");

        var query = new GameSM01PlatformLoginQuery(
             "4ucmqplxb029");

        var player = new PlayerInfo(
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY");

        _ = fakeGameProvider
            .GetGameProviderIdAsync(GameProviderType.SM01)
            .Returns(gameProviderId);

        var games = new GameModel[]
        {
            new GameModel(
                GameId: gameId,
                GameProviderId: gameProviderId,
                GameProviderCode: GameProviderType.SM01,
                GameExternalId: "sm01",
                GameStatus.Active,
                null,
                null,
                null,
                GameType.Sportsbook,
                GamePlatformType.DesktopAndMobile,
                SubGame: false,
                AvailableToBrand: true,
                HasTurnoverOnTieBet: false,
                Html5: true,
                null,
                GameCode: "sm01",
                "SM01",
                null,
                null)
        };

        _ = fakeGameProvidersQueryServerClient
            .GameQueryAsync(gameProviderId)
            .Returns(games.ToAsyncEnumerable());

        _ = fakeGameVerificationClient
            .VerifyPlayerGameUrlAsync(
                Arg.Is(query.PlayerCode),
                Arg.Is(gameId))
            .Returns(true);

        var loginInfo = new GameSM01PlatformLoginInfo(
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
            fakeGameProvidersQueryServerClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(loginInfo.MerchantId, actual.MerchantId);
        Assert.AreEqual(loginInfo.CustomerName, actual.CustomerName);
        Assert.AreEqual(loginInfo.Timestamp, actual.Timestamp);
        Assert.AreEqual(loginInfo.Sign, actual.Sign);
    }
}
