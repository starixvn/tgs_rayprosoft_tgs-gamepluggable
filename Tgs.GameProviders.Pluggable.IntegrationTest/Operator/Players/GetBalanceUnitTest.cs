using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players;

[TestClass]
public class GetBalanceUnitTest
{
    [TestMethod]
    public async Task GetBalanceAsync()
    {
        var playerCode = "4ucmqplxb029";
        var gameProviderCode = "VR";
        var gameProviderId = Guid.NewGuid();
        decimal gameProviderBalance = 0.88m;

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakePlayerBalanceGetService = Substitute.For<IGetService<PlayerBalanceQuery, decimal>>();

                var query = new PlayerBalanceQuery(
                    playerCode,
                    gameProviderCode);

                _ = fakePlayerBalanceGetService
                    .GetAsync(
                        Arg.Is(query))
                    .Returns(ValueTask.FromResult(gameProviderBalance));

                _ = services.AddSingleton(fakePlayerBalanceGetService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersOperatorPluggableClient(httpClient);

        var actual = await sut.GetBalanceAsync(playerCode, gameProviderCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameProviderBalance, actual);
    }
}
