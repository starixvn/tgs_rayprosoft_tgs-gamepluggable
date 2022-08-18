using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using Tgs.GameRounds.Common;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class RoundRepository : IRoundRepository
    {
        private readonly BetContext _betContext;
        private readonly ILogger<RoundRepository> _logger;
        private readonly ISerialNumberGenerator _serialNumberGenerator;

        public RoundRepository(
            BetContext betContext,
            ILogger<RoundRepository> logger,
            ISerialNumberGenerator serialNumberGenerator)
        {
            _betContext = betContext ?? throw new ArgumentNullException(nameof(betContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        }

        public IAsyncEnumerable<GameActionInfo> GameActionQueryAsync(Guid roundId)
            =>
            _betContext
            .GameActions
            .AsAsyncEnumerable()
            .Where(x => x.RoundId == roundId)
            .Select(x => new GameActionInfo(
                 x.Id,
                 x.RoundId,
                 x.PlayerCode,
                 x.ExternalTransactionId,
                 (QueryServer.Models.GameActionType)x.Type,
                 x.Amount,
                 x.Turnover,
                 x.WinLoss,
                 x.UnsettledBets,
                 x.Balance,
                 x.BetType,
                 x.ExternalBetId,
                 x.ExternalGameActionGroupId,
                 x.ReferencedGameActionId,
                 x.GameActionBatchId,
                 x.RecordedOn,
                 x.SentOn,
                 x.IsClosingRound,
                 x.ClosingRoundExplicitRequest,
                 x.PlayerIp,
                 x.ExtraData,
                 x.JackpotContribution,
                 x.StoredOn
                ));

        public async ValueTask<GameRoundInfo?> GetAsync(Guid roundId)
            => await _betContext
            .Rounds
            .Where(x => x.Id == roundId)
            .Select(x => new GameRoundInfo(
                x.Id,
                x.ExternalRoundId,
                x.GameProviderId,
                x.PlayerCode,
                x.GameId,
                x.CurrencyCode,
                (GameRoundStatus)x.Status,
                x.GameProviderConfigurationId,
                x.WalletId,
                x.CreatedOn,
                x.ClosedOn,
                x.UpdatedOn,
                x.BrandId,
                x.ExternalCreatedOn,
                x.ExternalClosedOn,
                x.BetAmount,
                x.ValidBetAmount,
                x.SettleAmount,
                x.Commission,
                x.Turnover,
                x.WinLoss,
                x.TieBetAdjustment,
                (GameRoundClientStatus)x.ClientStatus,
                x.ReopenTurnoverAdjustment,
                x.ReopenedOn,
                x.EffectiveBettingPercentage
                ))
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}
