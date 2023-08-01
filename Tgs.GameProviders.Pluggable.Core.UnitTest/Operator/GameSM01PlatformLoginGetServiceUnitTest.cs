using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

public class GameSM01PlatformLoginGetServiceUnitTest
{
    [Fact]
    public async Task Get_SM01_Platform_Login()
    {
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameCommandClient = Substitute.For<IGameCommandClient>();
        var fakePlayer = Substitute.For<IPlayer>();

        var gameId = 5200001;
        var gameProviderId = Guid.Parse("ba2997d9-b856-4589-91ae-5060a952cc3c");

        var query = new GameSM01PlatformLoginQuery(
             PlayerCode: "4ucmqplxb029");

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
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            Arg.Is(GameProviderType.SM01),
            cancellationToken: Arg.Any<CancellationToken>())
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
            .GameQueryAsync(
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(games.ToAsyncEnumerable());

        var loginInfo = new GameSM01PlatformLoginInfo(
            ApiUrl: "https://cn.ucfclive.com",
            MerchantId: "SQjCxm2cX5bahBta",
            CustomerName: "4ucmqplxb029",
            Timestamp: 1670390195123,
            Sign: "46C9EA4F03900B74379A15A56ED14229");

        _ = fakeGameUrl
            .GetGamePlatformLoginBySM01Async(
            playerCode: Arg.Is(player.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(loginInfo));

        var sut = new GameSM01PlatformLoginGetService(
            fakeGameUrl,
            fakeVerification,
            fakeGameProvider,
            fakeGameCommandClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(loginInfo.MerchantId, actual.MerchantId);
        Assert.Equal(loginInfo.CustomerName, actual.CustomerName);
        Assert.Equal(loginInfo.Timestamp, actual.Timestamp);
        Assert.Equal(loginInfo.Sign, actual.Sign);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerGameUrlAsync(
            player: Arg.Is(player),
            game: Arg.Is(games.ElementAt(0)),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
