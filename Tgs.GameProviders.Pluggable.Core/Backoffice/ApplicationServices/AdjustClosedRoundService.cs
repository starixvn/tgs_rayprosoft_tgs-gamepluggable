using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class AdjustClosedRoundService : ICommandService<AdjustClosedRoundCommand>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly ILogger<AdjustClosedRoundService> _logger;
        private readonly IRoundRepository _roundRepository;

        public AdjustClosedRoundService(
            ICashServiceClient cashServiceClient,
            ILogger<AdjustClosedRoundService> logger,
            IRoundRepository roundRepository)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask ExecuteAsync(AdjustClosedRoundCommand command, CancellationToken cancellationToken = default)
        {
            var gameRoundModel = await _roundRepository.GetAsync(command.RoundId).ConfigureAwait(false);

            if (gameRoundModel == null)
            {
                throw new Exception($"Game round number does not exist.({command.RoundId})");
            }

            if (await _roundRepository
                .AdjustClosedRoundAsync(
                    command.RoundId,
                    command.BetAmount,
                    command.ValidBetAmount,
                    command.SettleAmount,
                    command.Commission,
                    command.Turnover,
                    command.WinLoss,
                    command.ClientRoundStatus,
                    command.ExternalClosedOn,
                    command.OperatorId,
                    command.Remark)
                .ConfigureAwait(false))
            {
                if (command.AdjustAmount.HasValue)
                {
                    await _cashServiceClient.AdjustResettledBetManualAsync(
                          gameRoundModel.WalletId,
                          command.AdjustAmount.Value,
                          command.Remark,
                          gameRoundModel.RoundId.ToString(),
                          command.OperatorId);
                }
            }
            else
            {
                throw new Exception($"Failed to adjust closed game record.({command.RoundId})");
            }
        }
    }
}
