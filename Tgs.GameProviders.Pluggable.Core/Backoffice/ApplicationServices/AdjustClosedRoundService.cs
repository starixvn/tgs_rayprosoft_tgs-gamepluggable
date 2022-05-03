using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class AdjustClosedRoundService : ICommandService<AdjustClosedRoundCommand>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IRoundRepository _roundRepository;

        public AdjustClosedRoundService(
            ICashServiceClient cashServiceClient,
            IRoundRepository roundRepository)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
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
                    command.RoundClientStatus,
                    command.ExternalClosedOn,
                    command.OperatorId,
                    command.Remark)
                .ConfigureAwait(false))
            {
                if (command.AdjustAmount.HasValue)
                {
                    _ = await _cashServiceClient.AdjustResettledBetManualAsync(
                           gameRoundModel.WalletId,
                           command.AdjustAmount.Value,
                           command.Remark,
                           gameRoundModel.RoundId.ToString(),
                           command.OperatorId,
                           cancellationToken);
                }
            }
            else
            {
                throw new Exception($"Failed to adjust closed game record.({command.RoundId})");
            }
        }
    }
}
