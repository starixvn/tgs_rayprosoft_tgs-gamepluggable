using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class GameRoundClosedService : ICommandService<GameRoundClosedCommand>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly ILogger<GameRoundClosedService> _logger;
        private readonly IRoundRepository _roundRepository;

        public GameRoundClosedService(
            ICashServiceClient cashServiceClient,
            ILogger<GameRoundClosedService> logger,
            IRoundRepository roundRepository
            )
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask ExecuteAsync(GameRoundClosedCommand command, CancellationToken cancellationToken = default)
        {
            var gameRoundModel = await _roundRepository.GetAsync(command.RoundId).ConfigureAwait(false);
            if (gameRoundModel == null)
            {
                throw new Exception($"Game round number does not exist.({command.RoundId})");
            }
            if (gameRoundModel.Status == RoundStatus.Closed)
            {
                throw new Exception($"Game round number has been closed.({command.RoundId})");
            }

            if (await _roundRepository
                .ClosedGameRoundAsync(
                 command.RoundId,
                 command.BetAmount,
                 command.ValidBetAmount,
                 command.SettleAmount,
                 command.Commission,
                 command.Turnover,
                 command.WinLoss,
                 command.RoundClientStatus,
                 command.ExternalClosedOn,
                 command.OperatorId,
                 command.Remark
                 ).ConfigureAwait(false))
            {
                if (command.AdjustAmount.HasValue)
                {
                    _ = await _cashServiceClient.UnsettledBetManualAsync(
                          gameRoundModel.WalletId,
                          command.AdjustAmount.Value,
                          command.Remark,
                          gameRoundModel.RoundId.ToString(),
                          command.OperatorId);
                }
            }
            else
            {
                throw new Exception($"Failed to close game record.({command.RoundId})");
            }
        }
    }
}
