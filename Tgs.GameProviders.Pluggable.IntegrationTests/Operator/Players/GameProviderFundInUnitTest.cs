using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Services;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Operator.Players
{
    [TestClass]
    public class GameProviderFundInUnitTest
    {
        [TestMethod]
        public async Task GameProviderFundInAsync()
        {
            string gameProviderCode = GameProviderType.KY;
            string brandCode = "GPI";
            string playerCode = "4ucmqplxb029";
            string currencyCode = "CNY";
            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            var orderId = gameProviderFundTransferTransactionId.ToString("N");
            var amount = 1m;
            var gameProviderBalance = 1m;

            var transactionResponse = new TransactionResponse()
            {
                TransactionId = Guid.NewGuid(),
                Balance = 1001m
            };

            var playerModel = new PlayerModel(
                 Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled
                );

            var gameProviderModel = new GameProviderModel(
               Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
               GameProviderStatus.Active,
               string.Empty,
               true,
               true,
               false,
               0,
               0,
               (int)GameProviderUnique.KY,
               true,
               GameProviderType.KY,
               GameProviderType.KY,
               string.Empty
              );

            var gameProviderCurrencyModel = new GameProviderCurrencyModel(
                gameProviderModel.GameProviderId,
                playerModel.DefaultWalletCurrencyCode,
                "RMB");

            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakeLicenseeRepository = Substitute.For<ILicenseeRepository>();
            var faceLogger = NullLogger<PlayerVerifyService>.Instance;
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeResellerRepository = Substitute.For<IResellerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    _ = fakeGameProvidersClient
                    .PlayerBalanceAsync(
                       gameProviderCode,
                       playerCode)
                    .Returns(ValueTask.FromResult<decimal?>(gameProviderBalance));

                    _ = fakeGameProvidersClient
                    .PlayerBalanceAsync(
                        Arg.Is(gameProviderCode),
                        Arg.Is(playerCode)).Returns(ValueTask.FromResult<decimal?>(gameProviderBalance));

                    _ = fakeGameProvidersClient
                     .PlayerFundInAsync(
                        gameProviderCode,
                        brandCode,
                        playerCode,
                        currencyCode,
                        orderId,
                        amount).Returns(ValueTask.FromResult<string?>(orderId));

                    _ = fakeGameProvidersClient
                     .PlayerFundInAsync(
                        Arg.Is(gameProviderCode),
                        Arg.Is(brandCode),
                        Arg.Is(playerCode),
                        Arg.Is(currencyCode),
                        Arg.Is(orderId),
                        Arg.Is(amount)
                        ).Returns(ValueTask.FromResult<string?>(orderId));

                    _ = fakeCashServiceClient.GameProviderFundInAsync(
                        playerModel.DefaultWalletId,
                        amount,
                        $"【{ gameProviderModel.GameProviderCode}】->【Default】",
                        gameProviderFundTransferTransactionId.ToString()
                        ).Returns(ValueTask.FromResult(transactionResponse));

                    _ = fakeCashServiceClient.GameProviderFundInAsync(
                        Arg.Is(playerModel.DefaultWalletId),
                        Arg.Is(amount),
                        Arg.Is($"【{ gameProviderModel.GameProviderCode}】->【Default】"),
                        Arg.Is(gameProviderFundTransferTransactionId.ToString())
                       ).Returns(ValueTask.FromResult(transactionResponse));

                    services.AddSingleton(fakeGameProvidersClient);
                    services.AddSingleton(fakeCashServiceClient);
                    services.AddSingleton(fakeBrandRepository);
                    services.AddSingleton(fakeGameProviderRepository);
                    services.AddSingleton(fakeGameRepository);
                    services.AddSingleton(fakeLicenseeRepository);
                    services.AddSingleton(faceLogger);
                    services.AddSingleton(fakePlayerRepository);
                    services.AddSingleton(fakeResellerRepository);
                    services.AddSingleton(fakeGameProviderFundTransferTransactionRepository);
                    services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            _ = fakePlayerRepository
                .GetAsync(playerCode)
                .Returns(ValueTask.FromResult(playerModel));

            _ = fakeGameProviderRepository
                .GetAsync(gameProviderCode)
                .Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(
                Arg.Is(playerCode)).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository.GetAsync(
                Arg.Is(playerCode)).Returns(ValueTask.FromResult(playerModel));

            _ = fakeGameProviderRepository.GetCurrencyAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(gameProviderCurrencyModel.CurrencyCode))
                .Returns(ValueTask.FromResult<GameProviderCurrencyModel?>(gameProviderCurrencyModel));

            _ = fakeBrandRepository.GetCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                )
                .Returns(ValueTask.FromResult<string?>(playerModel.DefaultWalletCurrencyCode));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameProviderModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakeGameProviderFundTransferTransactionRepository.AddFundInAsync(
                Arg.Is(gameProviderModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletId),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(amount),
                Arg.Is(string.Empty)).Returns(ValueTask.FromResult(gameProviderFundTransferTransactionId));

            _ = fakeGameProviderFundTransferTransactionRepository.UpdateBySuccessAsync(
                Arg.Is(gameProviderFundTransferTransactionId),
                Arg.Is(transactionResponse.TransactionId),
                Arg.Is(orderId),
                Arg.Is(transactionResponse.Balance),
                Arg.Is(string.Empty)
                                    );

            var actual = await sut.GameProviderFundInAsync(
                playerCode,
                gameProviderCode,
                amount).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderFundTransferTransactionId, new Guid(actual.GameProviderTransactionId));
        }
    }
}
