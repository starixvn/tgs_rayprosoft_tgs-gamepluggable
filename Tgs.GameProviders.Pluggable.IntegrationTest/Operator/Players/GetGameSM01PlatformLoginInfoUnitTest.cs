using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players;

public class GetGameSM01PlatformLoginInfoUnitTest
{
    [Fact]
    public async Task GetGameSM01PlatformLoginInfoAsync()
    {
        var playerCode = "4ucmqplxb029";

        var loginInfo = new GameSM01PlatformLoginInfo(
                   ApiUrl: "https://cn.ucfclive.com",
                   MerchantId: "SQjCxm2cX5bahBta",
                   CustomerName: playerCode,
                   Timestamp: 1670390195123,
                   Sign: "46C9EA4F03900B74379A15A56ED14229");

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeGameUrlGetService = Substitute.For<IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>>();

                _ = fakeGameUrlGetService
                .GetAsync(
                    Arg.Any<GameSM01PlatformLoginQuery>(),
                    Arg.Any<CancellationToken>())
                 .Returns(ValueTask.FromResult<GameSM01PlatformLoginInfo>(loginInfo));

                _ = services.AddSingleton(fakeGameUrlGetService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersOperatorPluggableClient(httpClient);

        var actual = await sut.GetGameSM01PlatformLoginInfoAsync(playerCode);

        Assert.NotNull(actual);
        Assert.Equal(loginInfo.MerchantId, actual.MerchantId);
        Assert.Equal(loginInfo.CustomerName, actual.CustomerName);
        Assert.Equal(loginInfo.Timestamp, actual.Timestamp);
        Assert.Equal(loginInfo.Sign, actual.Sign);
    }
}
