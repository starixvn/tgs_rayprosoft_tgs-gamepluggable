using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

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
               (int)GameProviderUnique.KY,
               true,
               GameProviderType.KY,
               GameProviderType.KY,
               string.Empty
              );

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
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

                    services.AddSingleton(fakeGameProvidersClient);

                    var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

                    _ = fakeCashServiceClient.GameProviderFundOutAsync(
                        player.DefaultWalletId,
                        amount,
                        $"【Default】->【{ gameProvider.GameProviderCode}】",
                        gameProviderFundTransferTransactionId.ToString()
                        ).Returns(ValueTask.FromResult(transactionResponse));

                    _ = fakeCashServiceClient.GetBalanceAsync(
                        player.DefaultWalletId
                        ).Returns(ValueTask.FromResult(playerBalance));

                    services.AddSingleton(fakeCashServiceClient);

                    var fakePlayer = Substitute.For<IPlayerService>();

                    _ = fakePlayer
                        .GetAsync(playerCode)
                        .Returns(ValueTask.FromResult(player));

                    services.AddSingleton(fakePlayer);

                    var fakeBrand = Substitute.For<IBrandService>();

                    _ = fakeBrand
                        .GetStatusAsync(player.BrandId)
                        .Returns(ValueTask.FromResult(BrandStatus.Active));
                    _ = fakeBrand.GetCurrencyCodeAsync(
                        Arg.Is(player.BrandId),
                        Arg.Is(player.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult<string?>(player.DefaultWalletCurrencyCode));

                    services.AddSingleton(fakeBrand);

                    var fakeLicensee = Substitute.For<ILicenseeService>();

                    _ = fakeLicensee
                        .GetStatusAsync(player.LicenseeId)
                        .Returns(ValueTask.FromResult(LicenseeStatus.Active));

                    services.AddSingleton(fakeLicensee);

                    var fakeReseller = Substitute.For<IResellerService>();

                    _ = fakeReseller
                        .GetStatusAsync(player.ResellerId)
                        .Returns(ValueTask.FromResult(ResellerStatus.Active));

                    services.AddSingleton(fakeReseller);

                    var fakeGameProvider = Substitute.For<IGameProviderService>();

                    _ = fakeGameProvider.GetAsync(
                        Arg.Is(gameProviderCode)).Returns(ValueTask.FromResult(gameProvider));

                    _ = fakeGameProvider.GetCurrencyCodeAsync(
                        Arg.Is(gameProvider.GameProviderId),
                        Arg.Is(player.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult<string?>(player.DefaultWalletCurrencyCode));

                    services.AddSingleton(fakeGameProvider);

                    var fakePlayerVerify = Substitute.For<IPlayerVerifyHanlde>();
                    _ = fakePlayerVerify.IsPlayerEnableAsync(
                        player).Returns(ValueTask.FromResult(true));

                    _ = fakePlayerVerify.IsPlayerGameProviderActiveAsync(
                        player.BrandId,
                        gameProvider).Returns(ValueTask.FromResult(true));

                    _ = fakePlayerVerify.IsPlayerGameProviderCurrencyAsync(
                        player.BrandId,
                        gameProvider.GameProviderId,
                        player.DefaultWalletCurrencyCode).Returns(ValueTask.FromResult(true));

                    services.AddSingleton(fakePlayerVerify);

                    var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();

                    _ = fakeGameProviderFundTransferTransactionRepository.AddFundOutAsync(
                        Arg.Is(gameProvider.GameProviderId),
                        Arg.Is(player.DefaultWalletId),
                        Arg.Is(player.PlayerCode),
                        Arg.Is(gameProvider.GameProviderCode),
                        Arg.Is(amount * -1),
                        Arg.Is(string.Empty)).Returns(ValueTask.FromResult(gameProviderFundTransferTransactionId));

                    _ = fakeGameProviderFundTransferTransactionRepository.UpdateBySuccessAsync(
                        Arg.Is(gameProviderFundTransferTransactionId),
                        Arg.Is(transactionResponse.TransactionId),
                        Arg.Is(orderId),
                        Arg.Is(transactionResponse.Balance),
                        Arg.Is(string.Empty));

                    services.AddSingleton(fakeGameProviderFundTransferTransactionRepository);
                    services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            var actual = await sut.GameProviderFundOutAsync(
                playerCode,
                gameProviderCode,
                amount).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderFundTransferTransactionId, new Guid(actual.GameProviderTransactionId));
        }
    }
}
