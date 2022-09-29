using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundClosedService : ICommandService<GameRoundClosedCommand>
{
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IGameRoundsClient _gameRoundsClient;

    public GameRoundClosedService(
        ICashServiceClient cashServiceClient,
        IGameRoundsClient gameRoundsClient)
    {
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        _gameRoundsClient = gameRoundsClient ?? throw new ArgumentNullException(nameof(gameRoundsClient));
    }

    public async ValueTask ExecuteAsync(GameRoundClosedCommand command, CancellationToken cancellationToken = default)
    {
        var gameRoundModel = await _gameRoundsClient
            .GetGameRoundAsync(command.RoundId, cancellationToken)
            .ConfigureAwait(false);

        if (gameRoundModel == null)
        {
            throw new Exception($"Game round number does not exist.({command.RoundId})");
        }

        if (gameRoundModel.Data!.Status == GameRoundStatus.Closed)
        {
            throw new Exception($"Game round number has been closed.({command.RoundId})");
        }

        var gameRoundResponse = await _gameRoundsClient
            .ManualCloseRoundAsync(command.RoundId, new GameRounds.Common.ViewModels.ManualCloseRoundViewModel
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
            }, cancellationToken)
            .ConfigureAwait(false);

        if (gameRoundResponse.Success)
        {
            if (command.AdjustAmount.HasValue)
            {
                _ = await _cashServiceClient.UnsettledBetManualAsync(
                      gameRoundModel.Data.WalletId,
                      command.AdjustAmount.Value,
                      command.Remark,
                      gameRoundModel.Data.RoundId.ToString(),
                      command.OperatorId,
                      cancellationToken);
            }
        }
        else
        {
            throw new Exception($"Failed to close game record.({command.RoundId})");
        }
    }
}
