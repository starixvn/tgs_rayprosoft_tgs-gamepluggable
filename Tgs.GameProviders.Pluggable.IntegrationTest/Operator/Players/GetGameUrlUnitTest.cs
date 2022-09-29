using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players;

[TestClass]
public class GetGameUrlUnitTest
{
    [TestMethod]
    public async Task GetGameUrlAsync()
    {
        string playerCode = "4ucmqplxb029";
        Guid loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        bool isMobile = platformType == GameProvidersClientPlatformType.Mobile;
        int gameId = 1000001;
        string language = "zh-CN";
        string ipAddress = "127.0.0.1";
        Uri backUrl = new("https://dev-t1.rpgp.cc");
        Uri gameurl = new("https://www.google.com/");

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
              {
                  var fakeGameUrlGetService = Substitute.For<IGetService<GameUrlQuery, Uri>>();

                  var query = new GameUrlQuery(
                      playerCode,
                      loginRecordId,
                      platformType,
                      gameId,
                      language,
                      ipAddress,
                      backUrl);
                  _ = fakeGameUrlGetService
                   .GetAsync(
                      Arg.Is(query))
                   .Returns(ValueTask.FromResult<Uri>(gameurl));

                  _ = services.AddSingleton(fakeGameUrlGetService);
              });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersOperatorPluggableClient(httpClient);

        var actual = await sut.GetGameUrlAsync(
            playerCode,
            loginRecordId,
            isMobile,
            gameId,
            language,
            ipAddress,
            backUrl).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameurl, actual);
    }
}
