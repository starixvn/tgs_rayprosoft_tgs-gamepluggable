using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Rounds
{
    [TestClass]
    public class GameRoundGetUnitTest
    {
        [TestMethod]
        public async Task RoundGameRoundGetAsync()
        {
            var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");

            var gameRoundModel = new GameRoundInfo(RoundId: Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02"),
               ExternalRoundId: "50-1636345294-40676339-2",
               GameProviderId: Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
               PlayerCode: "4uf6j0t01809",
               GameId: 1000015,
               CurrencyCode: "CNY",
               Status: GameRoundStatus.Open,
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
               ClientStatus: GameRoundClientStatus.Open,
               ReopenTurnoverAdjustment: null,
               ReopenedOn: null,
               EffectiveBettingPercentage: null);

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    var fakeGameRoundGetService = Substitute.For<IGetService<Guid, GameRoundInfo>>();

                    _ = fakeGameRoundGetService
                        .GetAsync(
                            Arg.Is(roundId))
                        .Returns(ValueTask.FromResult(gameRoundModel));

                    _ = services.AddSingleton(fakeGameRoundGetService);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            var actual = await sut
                .GameRoundGetAsync(roundId)
                .ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
        }
    }
}
