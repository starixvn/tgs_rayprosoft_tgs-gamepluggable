using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class ClosedGameRoundService : ICommandService<ClosedGameRoundCommand>
    {
        private readonly ILogger<ClosedGameRoundService> _logger;
        private readonly IRoundOperationRecordsRepository _roundOperationRecordsRepository;
        private readonly IRoundRepository _roundRepository;

        public ClosedGameRoundService(
              ILogger<ClosedGameRoundService> logger,
              IRoundOperationRecordsRepository roundOperationRecordsRepository,
              IRoundRepository roundRepository
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundOperationRecordsRepository = roundOperationRecordsRepository ?? throw new ArgumentNullException(nameof(roundOperationRecordsRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask ExecuteAsync(ClosedGameRoundCommand command, CancellationToken cancellationToken = default)
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

            await _roundRepository
                .ClosedGameRoundAsync(
                command.RoundId,
                command.BetAmount,
                command.ValidBetAmount,
                command.SettleAmount,
                command.Commission,
                command.Turnover,
                command.WinLoss,
                command.ClientRoundStatus,
                command.ExternalClosedOn
                )
                .ConfigureAwait(false);

            var newGameRoundModel = await _roundRepository.GetAsync(command.RoundId).ConfigureAwait(false);

            await _roundOperationRecordsRepository
                .AddAsync(
                command.OperatorId,
                command.RoundId,
                JsonSerializer.Serialize(gameRoundModel),
                JsonSerializer.Serialize(newGameRoundModel),
                command.Remark)
                .ConfigureAwait(false);
        }
    }
}
