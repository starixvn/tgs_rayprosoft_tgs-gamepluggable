using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;
using RoundClientStatus = Tgs.GameProviders.QueryServer.Models.RoundClientStatus;
using RoundStatus = Tgs.GameProviders.QueryServer.Models.RoundStatus;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Rounds
{
    [TestClass]
    public class GameRoundDetailGetUnitTest
    {
        [TestMethod]
        public async Task GameRoundDetailGetAsync()
        {
            var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
            var language = "zh-CN";
            var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");
            var gameRoundModel = new GameRoundInfo(
                RoundId: Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02"),
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
                EffectiveBettingPercentage: null);
            var gameProvider = new GameProviderInfo(
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
                 GameProviderUnique.KY.ToString(),
                 string.Empty
                );
            var roundDetailResult = new RoundDetailResult()
            {
                Data = null
            };

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                  {
                      var fakeRoundRepository = Substitute.For<IRoundRepository>();

                      _ = fakeRoundRepository
                          .GetAsync(roundId)
                          .Returns(ValueTask.FromResult<GameRoundInfo?>(gameRoundModel));

                      _ = fakeRoundRepository
                          .GetAsync(roundId)
                          .Returns(ValueTask.FromResult<GameRoundInfo?>(gameRoundModel));

                      _ = services.AddSingleton(fakeRoundRepository);

                      var fakeGameProvider = Substitute.For<IGameProviderService>();

                      _ = fakeGameProvider
                          .GetAsync(gameProviderId)
                          .Returns(ValueTask.FromResult(gameProvider));

                      _ = fakeGameProvider
                          .GetAsync(gameProviderId)
                          .Returns(ValueTask.FromResult(gameProvider));

                      _ = services.AddSingleton(fakeGameProvider);

                      var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                      _ = fakeGameProvidersClient
                         .RoundDetailAsync(gameProvider.GameProviderCode, roundId, language)
                         .Returns(ValueTask.FromResult<RoundDetailResult?>(roundDetailResult));
                      _ = fakeGameProvidersClient
                         .RoundDetailAsync(gameProvider.GameProviderCode, roundId, language)
                         .Returns(ValueTask.FromResult<RoundDetailResult?>(roundDetailResult));

                      _ = services.AddSingleton(fakeGameProvidersClient);
                  });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            var actual = await sut
                .GameRoundDetailGetAsync(roundId, language)
                .ConfigureAwait(false);
            Assert.IsNotNull(actual);
        }
    }
}
