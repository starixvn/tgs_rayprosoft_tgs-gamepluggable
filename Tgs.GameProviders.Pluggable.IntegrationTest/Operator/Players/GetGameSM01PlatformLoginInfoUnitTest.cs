using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players;

[TestClass]
public class GetGameSM01PlatformLoginInfoUnitTest
{
    [TestMethod]
    public async Task GetGameSM01PlatformLoginInfoAsync()
    {
        var playerCode = "4ucmqplxb029";
        var gameId = 1000001;

        var loginInfo = new GameSM01PlatformLoginInfo(
            ApiUrl: "https://cn.ucfclive.com",
            MerchantId: "SQjCxm2cX5bahBta",
            CustomerName: playerCode,
            Timestamp: 1670390195123,
            Sign: "46C9EA4F03900B74379A15A56ED14229");

        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
        var fakeGameSM01PlatformLoginGetService = Substitute.For<IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>>();

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
              {
                  _ = services.AddTransient(_ => fakeGameVerificationClient);

                  _ = services.AddTransient(_ => fakeGameSM01PlatformLoginGetService);
              });
        });

        _ = fakeGameVerificationClient.VerifyPlayerGameUrlAsync(playerCode, gameId).Returns(ValueTask.FromResult(true));

        _ = fakeGameSM01PlatformLoginGetService.GetAsync(Arg.Any<GameSM01PlatformLoginQuery>()).Returns(ValueTask.FromResult(loginInfo));

        var httpClient = web.CreateClient();

        var sut = new GameProvidersOperatorPluggableClient(httpClient);

        var actual = await sut
            .GetGameSM01PlatformLoginInfoAsync(playerCode)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(loginInfo.MerchantId, actual.MerchantId);
        Assert.AreEqual(loginInfo.CustomerName, actual.CustomerName);
        Assert.AreEqual(loginInfo.Timestamp, actual.Timestamp);
        Assert.AreEqual(loginInfo.Sign, actual.Sign);
    }
}
