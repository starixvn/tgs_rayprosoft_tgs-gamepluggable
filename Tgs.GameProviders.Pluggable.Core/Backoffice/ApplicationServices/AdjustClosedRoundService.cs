using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class AdjustClosedRoundService(
    ICashServiceClient cashServiceClient,
    IGameRoundsClient gameRoundsClient) : ICommandService<AdjustClosedRoundCommand>
{
    public async ValueTask ExecuteAsync(AdjustClosedRoundCommand command, CancellationToken cancellationToken = default)
    {
        var gameRoundModel = await gameRoundsClient
            .GetGameRoundAsync(
            roundId: command.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game round number does not exist.({command.RoundId})");

        var adjustClosedRound = new AdjustClosedRoundViewModel
        {
            ClientStatus = command.RoundClientStatus,
            ExternalClosedOn = command.ExternalClosedOn,
            BetAmount = command.BetAmount,
            SettleAmount = command.SettleAmount,
            ValidBetAmount = command.ValidBetAmount,
            Commission = command.Commission,
            Turnover = command.Turnover,
            CompanyWinLoss = command.WinLoss,
            OperatorId = command.OperatorId,
            Remark = command.Remark
        };
        await gameRoundsClient
            .AdjustClosedRoundAsync(
            command.RoundId,
            source: adjustClosedRound,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (command.AdjustAmount.HasValue)
        {
            _ = await cashServiceClient
                .AdjustResettledBetManualAsync(
                walletId: gameRoundModel.WalletId,
                amount: command.AdjustAmount.Value,
                description: command.Remark,
                externalId: gameRoundModel.RoundId.ToString(),
                createdByUserId: command.OperatorId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
