using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Operator.Players
{
    [TestClass]
    public class GameProviderFundOutUnitTest
    {
        [TestMethod]
        public async Task GameProviderFundOutAsync()
        {
            string gameProviderCode = GameProviderType.KY;
            string brandCode = "GPI";
            string playerCode = "4ucmqplxb029";
            string currencyCode = "CNY";
            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            var orderId = gameProviderFundTransferTransactionId.ToString("N");
            var amount = 1m;
            var playerBalance = 1000m;
            var gameProviderBalance = 1m;

            var transactionResponse = new TransactionResponse()
            {
                TransactionId = Guid.NewGuid(),
                Balance = 1001m
            };

            var player = new PlayerInfo(
                 Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                QueryServer.Models.PlayerStatus.Enabled
                );

            var gameProvider = new GameProviderInfo(
               Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
               QueryServer.Models.GameProviderStatus.Active,
               string.Empty,
               true,
               true,
               false,
               0,
               0,
               (int)10,
               true,
               GameProviderType.KY,
               GameProviderType.KY,
               string.Empty
              );

            var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                  {
                      _ = services.AddSingleton(fakeGameFundTransferClient);

                      var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                      _ = fakeGameProvidersClient
                      .PlayerBalanceAsync(
                         gameProviderCode,
                         playerCode)
                      .Returns(ValueTask.FromResult<decimal?>(gameProviderBalance));

                      _ = fakeGameProvidersClient
                       .PlayerFundOutAsync(
                          gameProviderCode,
                          brandCode,
                          playerCode,
                          currencyCode,
                          orderId,
                          amount).Returns(ValueTask.FromResult<string?>(orderId));

                      _ = services.AddSingleton(fakeGameProvidersClient);

                      var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

                      _ = fakeCashServiceClient.GameProviderFundOutAsync(
                          player.DefaultWalletId,
                          amount,
                          $"【Default】->【{gameProvider.GameProviderCode}】",
                          gameProviderFundTransferTransactionId.ToString()
                          ).Returns(ValueTask.FromResult(transactionResponse));

                      _ = fakeCashServiceClient.GetBalanceAsync(
                          player.DefaultWalletId
                          ).Returns(ValueTask.FromResult(playerBalance));

                      _ = services.AddSingleton(fakeCashServiceClient);

                      var fakePlayer = Substitute.For<IPlayerService>();

                      _ = fakePlayer
                          .GetAsync(playerCode)
                          .Returns(ValueTask.FromResult(player));

                      _ = services.AddSingleton(fakePlayer);

                      var fakeGameProvider = Substitute.For<IGameProviderService>();

                      _ = fakeGameProvider.GetAsync(
                          Arg.Is(gameProviderCode)).Returns(ValueTask.FromResult(gameProvider));

                      _ = services.AddSingleton(fakeGameProvider);

                      var fakeGameVerification = Substitute.For<IGameVerificationClient>();

                      _ = fakeGameVerification.VerifyPlayerFundAsync(
                          player.PlayerCode, gameProvider.GameProviderId).Returns(true);

                      _ = services.AddSingleton(fakeGameVerification);
                  });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);
            await sut.GameProviderFundOutAsync(
                playerCode,
                gameProviderCode,
                amount).ConfigureAwait(false);

            var request = new GameFundTransferFundRequest()
            {
                GameProviderId = gameProvider.GameProviderId,
                GameProviderCode = gameProvider.GameProviderCode,
                BrandCode = player.BrandCode,
                PlayerCode = player.PlayerCode,
                CurrencyCode = player.DefaultWalletCurrencyCode,
                WalletId = player.DefaultWalletId,
                Amount = amount
            };
            _ = fakeGameFundTransferClient.Received(1).FundOutAsync(request);
        }
    }
}
