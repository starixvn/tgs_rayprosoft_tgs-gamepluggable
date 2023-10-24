using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players;

public class GameProviderFundOutUnitTest
{
    [Fact]
    public async Task GameProviderFundOutAsync()
    {
        string gameProviderCode = GameProviderType.Kaiyuan;
        string playerCode = "4ucmqplxb029";
        var gameProviderFundTransferTransactionId = Guid.NewGuid();
        var orderId = gameProviderFundTransferTransactionId.ToString("N");
        var amount = 1m;

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
              {
                  var fakeFundInCommandService = Substitute.For<ICommandService<FundOutCommand>>();

                  var command = new FundOutCommand(
                      playerCode,
                      gameProviderCode,
                      amount);
                  _ = fakeFundInCommandService
                  .ExecuteAsync(Arg.Is(command));

                  _ = services.AddSingleton(fakeFundInCommandService);
              });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersOperatorPluggableClient(httpClient);

        await sut.GameProviderFundOutAsync(
            playerCode: playerCode,
            gameProviderCode: gameProviderCode,
            amount: amount);
    }
}
