using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.ViewModels;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Rounds
{
    [TestClass]
    public class GameRoundClosedUnitTest
    {
        [TestMethod]
        public async Task Round_GameAction_QueryAsync()
        {
            var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
            var betAmount = 0.060000m;
            var validBetAmount = 0.060000m;
            var settleAmount = 0.00m;
            var commission = 0.0m;
            var turnover = 0.06m;
            var winLoss = -0.06m;
            var clientRoundStatus = ClientRoundStatus.Loss;
            var externalClosedOn = DateTimeOffset.Parse("2021-06-03 13:57:05.000 +0800");
            var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
            var remark = "測試調整";

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeRoundRepository = Substitute.For<IRoundRepository>();
                    var fakeRoundOperationRecordsRepository = Substitute.For<IRoundOperationRecordsRepository>();

                    fakeRoundRepository.ClosedGameRoundAsync(
                        roundId,
                        betAmount,
                        validBetAmount,
                        settleAmount,
                        commission,
                        turnover,
                        winLoss,
                        clientRoundStatus,
                        externalClosedOn);

                    fakeRoundRepository.ClosedGameRoundAsync(
                        Arg.Is(roundId),
                        Arg.Is(betAmount),
                        Arg.Is(validBetAmount),
                        Arg.Is(settleAmount),
                        Arg.Is(commission),
                        Arg.Is(turnover),
                        Arg.Is(winLoss),
                        Arg.Is(clientRoundStatus),
                        Arg.Is(externalClosedOn));

                    fakeRoundOperationRecordsRepository.AddAsync(
                        operatorId,
                        roundId,
                        string.Empty,
                        string.Empty,
                        remark);

                    fakeRoundOperationRecordsRepository.AddAsync(
                        Arg.Is(operatorId),
                        Arg.Is(roundId),
                        Arg.Is(string.Empty),
                        Arg.Is(string.Empty),
                        Arg.Is(remark));

                    services.AddSingleton(fakeRoundRepository);
                    services.AddSingleton(fakeRoundOperationRecordsRepository);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            var viewModel = new GameRoundClosedViewModel()
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
                WinLoss = winLoss
            };

            var actual = sut.GameRoundClosedAsync(roundId, viewModel);
            Assert.IsNotNull(actual);
        }
    }
}
