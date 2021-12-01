using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Core;
using TGS.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Rounds
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
            var gameRoundModel = new GameRoundModel(RoundId: Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02"),
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
               ClientStatus: ClientRoundStatus.Open,
               ReopenTurnoverAdjustment: null,
               ReopenedOn: null,
               EffectiveBettingPercentage: null);
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
                 GameProviderUnique.KY.ToString(),
                 string.Empty
                );
            var roundDetailResult = new RoundDetailResult()
            {
                GameProviderName = gameProviderModel.GameProviderCode,
                GameName = "百家樂",
                ExternalRoundId = gameRoundModel.ExternalRoundId,
                BetAmount = gameRoundModel.BetAmount,
                Turnover = gameRoundModel.Turnover,
                Winloss = gameRoundModel.WinLoss,
                GameResult = gameRoundModel.ClientStatus.ToString(),
                Data = null
            };

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeRoundRepository = Substitute.For<IRoundRepository>();


                    _ = fakeRoundRepository
                        .GetAsync(roundId)
                        .Returns(ValueTask.FromResult<GameRoundModel?>(gameRoundModel));

                    _ = fakeRoundRepository
                        .GetAsync(roundId)
                        .Returns(ValueTask.FromResult<GameRoundModel?>(gameRoundModel));

                    var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

                    _ = fakeGameProviderRepository
                        .GetAsync(gameProviderId)
                        .Returns(ValueTask.FromResult<GameProviderModel?>(gameProviderModel));

                    _ = fakeGameProviderRepository
                        .GetAsync(gameProviderId)
                        .Returns(ValueTask.FromResult<GameProviderModel?>(gameProviderModel));

                    var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                    _ = fakeGameProvidersClient
                       .RoundDetailAsync(gameProviderModel.GameProviderCode, roundId, language)
                       .Returns(ValueTask.FromResult<RoundDetailResult?>(roundDetailResult));
                    _ = fakeGameProvidersClient
                       .RoundDetailAsync(gameProviderModel.GameProviderCode, roundId, language)
                       .Returns(ValueTask.FromResult<RoundDetailResult?>(roundDetailResult));

                    services.AddSingleton(fakeRoundRepository);
                    services.AddSingleton(fakeGameProviderRepository);
                    services.AddSingleton(fakeGameProvidersClient);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersPluggableClient(httpClient);

            var actual = await sut
                .GameRoundDetailGetAsync(roundId, language)
                .ConfigureAwait(false);
            Assert.IsNotNull(actual);
        }
    }
}
