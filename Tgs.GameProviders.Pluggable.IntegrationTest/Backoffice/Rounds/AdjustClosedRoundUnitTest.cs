using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Rounds;

public class AdjustClosedRoundUnitTest
{
    [Fact]
    public async Task Adjust_Closed_Game_Round()
    {
        var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");

        var model = new AdjustClosedRoundViewModel()
        {
            BetAmount = 0.060000m,
            ClientRoundStatus = GameRoundClosedClientRoundStatus.Loss,
            Commission = 0.0m,
            ExternalClosedOn = DateTimeOffset.Parse("2021-06-03 13:57:05.000 +0800"),
            OperatorId = Guid.Parse("ff73bf6f-637e-4575-3cfd-08d9262ce1a5"),
            Remark = "測試調整Closed Round",
            SettleAmount = 0.00m,
            Turnover = 0.06m,
            ValidBetAmount = 0.060000m,
            WinLoss = -0.06m,
            AdjustAmount = 100m
        };

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeAdjustClosedRound = Substitute.For<ICommandService<GameRoundResettleCommand>>();

                var command = new GameRoundResettleCommand(
                    RoundId: roundId,
                    BetAmount: model.BetAmount,
                    ValidBetAmount: model.ValidBetAmount,
                    SettleAmount: model.SettleAmount,
                    Commission: model.Commission,
                    Turnover: model.Turnover,
                    CompanyWinLoss: model.WinLoss,
                    RoundClientStatus: (GameRoundClientStatus)model.ClientRoundStatus,
                    ExternalClosedOn: model.ExternalClosedOn,
                    OperatorId: model.OperatorId,
                    Remark: model.Remark,
                    AdjustAmount: model.AdjustAmount);

                _ = fakeAdjustClosedRound
                .ExecuteAsync(
                    command)
                .Returns(ValueTask.CompletedTask);

                _ = services.AddSingleton(fakeAdjustClosedRound);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersBackofficePluggableClient(httpClient);

        await sut.AdjustClosedRoundAsync(
            roundId: roundId,
            viewModel: model);
    }
}
