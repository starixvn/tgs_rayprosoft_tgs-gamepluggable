using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Backoffice
{
    [TestFixture]
    public class AdjustClosedRoundServiceUnitTest
    {
        [Test]
        public async Task Can_Resettle_Closed_Game_Round()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeRoundRepository = Substitute.For<IRoundRepository>();

            var sut = new AdjustClosedRoundService(
                fakeCashServiceClient,
                fakeRoundRepository);

            var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
            var betAmount = 0.060000m;
            var validBetAmount = 0.060000m;
            var settleAmount = 0.00m;
            var commission = 0.0m;
            var turnover = 0.06m;
            var winLoss = -0.06m;
            var roundClientStatus = RoundClientStatus.Loss;
            var externalClosedOn = DateTimeOffset.Parse("2021-06-03 13:57:05.000 +0800");
            var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
            var remark = "測試調整Closed Round";
            var adjustAmount = 100m;

            var gameRoundModel = new GameRoundInfo
            (
                RoundId: roundId,
                ExternalRoundId: "50-1636345294-40676339-2",
                GameProviderId: Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                PlayerCode: "4uf6j0t01809",
                GameId: 1000015,
                CurrencyCode: "CNY",
                Status: RoundStatus.Open,
                GameProviderConfigurationId: Guid.Parse("fa0d7706-f910-4e98-bf71-bd7084140304"),
                WalletId: Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9"),
                CreatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
                BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
                ExternalClosedOn: null,
                BetAmount: 0.060000m,
                ValidBetAmount: 0.060000m,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                WinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: RoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null
            );

            _ = fakeRoundRepository
                .GetAsync(roundId)
                .Returns(gameRoundModel);

            _ = fakeRoundRepository
                .AdjustClosedRoundAsync(
                    roundId,
                    betAmount,
                    validBetAmount,
                    settleAmount,
                    commission,
                    turnover,
                    winLoss,
                    roundClientStatus,
                    externalClosedOn,
                    operatorId,
                    remark)
                .Returns(true);

            var command = new AdjustClosedRoundCommand
            {
                RoundId = roundId,
                BetAmount = betAmount,
                ValidBetAmount = validBetAmount,
                SettleAmount = settleAmount,
                Commission = commission,
                Turnover = turnover,
                WinLoss = winLoss,
                RoundClientStatus = roundClientStatus,
                ExternalClosedOn = externalClosedOn,
                OperatorId = operatorId,
                Remark = remark,
                AdjustAmount = adjustAmount
            };

            await sut
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            _ = fakeRoundRepository
                .Received(1)
                .AdjustClosedRoundAsync(
                    roundId,
                    betAmount,
                    validBetAmount,
                    settleAmount,
                    commission,
                    turnover,
                    winLoss,
                    roundClientStatus,
                    externalClosedOn,
                    operatorId,
                    remark);

            var walletId = Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9");

            _ = fakeCashServiceClient
                .Received(1)
                .AdjustResettledBetManualAsync(
                    Arg.Is(walletId),
                    Arg.Is(adjustAmount),
                    Arg.Is(remark),
                    Arg.Is(roundId.ToString()),
                    Arg.Is(operatorId));
        }
    }
}
