using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Rounds;

public class AddSystemRoundUnitTest
{
    [Fact]
    public async Task Add_System_Round()
    {
        var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
        var walletId = Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9");
        var betAmount = 0.060000m;
        var validBetAmount = 0.060000m;
        var settleAmount = 0.00m;
        var commission = 0.0m;
        var turnover = 0.06m;
        var winLoss = -0.06m;
        var clientRoundStatus = GameRoundClientStatus.Loss;
        var externalCreatedOn = DateTimeOffset.Parse("2021-06-01 13:57:05.000 +0800");
        var externalClosedOn = DateTimeOffset.Parse("2021-06-03 13:57:05.000 +0800");
        var operatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5");
        var remark = "測試調整Closed Round";
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var playerCode = "4ucmqplxb029";
        var gameProviderId = Guid.Parse("579d6da3-b0ed-4e3f-bd4a-434a98efbd68");

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeGameRoundsClient = Substitute.For<IGameRoundsClient>();
                var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

                _ = fakeGameRoundsClient.AddCompanyRoundAsync(new GameRounds.Common.ViewModels.AddCompanyRoundViewModel
                {
                    BrandId = brandId,
                    PlayerCode = playerCode,
                    WalletId = walletId,
                    GameId = 123,
                    GameProviderId = gameProviderId,
                    GameProviderConfigurationId = Guid.NewGuid(),
                    ExternalRoundId = "exRoundId",
                    ClientStatus = clientRoundStatus,
                    ExternalCreatedOn = externalCreatedOn,
                    ExternalClosedOn = externalClosedOn,
                    CurrencyCode = "CNY",
                    BetAmount = betAmount,
                    SettleAmount = settleAmount,
                    ValidBetAmount = validBetAmount,
                    Turnover = turnover,
                    Commission = commission,
                    CompanyWinLoss = winLoss,
                    OperatorId = operatorId,
                    Remark = remark
                });

                _ = services.AddSingleton(fakeGameRoundsClient);
                _ = services.AddSingleton(fakeCashServiceClient);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersBackofficePluggableClient(httpClient);

        var model = new AddCompanyRoundViewModel
        {
            BrandId = brandId,
            PlayerCode = playerCode,
            WalletId = walletId,
            GameId = 123,
            GameProviderId = gameProviderId,
            ExternalRoundId = "exRoundId",
            ClientRoundStatus = (GameRoundClosedClientRoundStatus)clientRoundStatus,
            ExternalCreatedOn = externalCreatedOn,
            ExternalClosedOn = externalClosedOn,
            CurrencyCode = "CNY",
            BetAmount = betAmount,
            SettleAmount = settleAmount,
            ValidBetAmount = validBetAmount,
            Turnover = turnover,
            Commission = commission,
            CompanyWinLoss = winLoss,
            OperatorId = operatorId,
            Remark = remark
        };

        await sut
            .AddCompanyRoundAsync(
                viewModel: model)
            .ConfigureAwait(false);
    }
}
