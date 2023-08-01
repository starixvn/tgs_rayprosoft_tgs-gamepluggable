using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players;

public class GetGameUrlUnitTest
{
    [Fact]
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
                      PlayerCode: playerCode,
                      LoginRecordId: loginRecordId,
                      PlatformType: platformType,
                      GameId: gameId,
                      Language: language,
                      IpAddress: ipAddress,
                      BackUrl: backUrl);

                  _ = fakeGameUrlGetService
                   .GetAsync(
                      query: Arg.Is(query),
                      cancellationToken: Arg.Any<CancellationToken>())
                   .Returns(ValueTask.FromResult<Uri>(gameurl));

                  _ = services.AddSingleton(fakeGameUrlGetService);
              });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersOperatorPluggableClient(httpClient);

        var actual = await sut
            .GetGameUrlAsync(
            playerCode,
            loginRecordId,
            isMobile,
            gameId,
            language,
            ipAddress,
            backUrl)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameurl, actual);
    }
}
