using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Rounds
{
    [TestClass]
    public class AdjustClosedRoundUnitTest
    {
        [TestMethod]
        public async Task Adjust_Closed_Game_Round()
        {
            var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
            var walletId = Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9");
            var betAmount = 0.060000m;
            var validBetAmount = 0.060000m;
            var settleAmount = 0.00m;
            var commission = 0.0m;
            var turnover = 0.06m;
            var winLoss = -0.06m;
            var clientRoundStatus = RoundClientStatus.Loss;
            var externalClosedOn = DateTimeOffset.Parse("2021-06-03 13:57:05.000 +0800");
            var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
            var remark = "測試調整Closed Round";
            var adjustAmount = 100m;
            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    var fakeRoundRepository = Substitute.For<IRoundRepository>();
                    var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

                    _ = fakeRoundRepository.AdjustClosedRoundAsync(
                        roundId,
                        betAmount,
                        validBetAmount,
                        settleAmount,
                        commission,
                        turnover,
                        winLoss,
                        clientRoundStatus,
                        externalClosedOn,
                        operatorId,
                        remark);

                    _ = fakeCashServiceClient
                    .AdjustResettledBetManualAsync(
                        walletId,
                        adjustAmount,
                        remark,
                        roundId.ToString(),
                        operatorId
                        );

                    _ = services.AddSingleton(fakeRoundRepository);
                    _ = services.AddSingleton(fakeCashServiceClient);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            var viewModel = new AdjustClosedRoundViewModel()
            {
                BetAmount = betAmount,
                ClientRoundStatus = (GameRoundClosedClientRoundStatus)(int)clientRoundStatus,
                Commission = commission,
                ExternalClosedOn = externalClosedOn,
                OperatorId = operatorId,
                Remark = remark,
                SettleAmount = settleAmount,
                Turnover = turnover,
                ValidBetAmount = validBetAmount,
                WinLoss = winLoss,
                AdjustAmount = adjustAmount
            };

            var actual = sut.AdjustClosedRoundAsync(roundId, viewModel);
            Assert.IsNotNull(actual);
        }
    }
}
