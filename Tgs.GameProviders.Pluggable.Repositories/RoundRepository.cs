using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class RoundRepository : IRoundRepository
    {
        private readonly BetContext _betContext;
        private readonly ILogger<RoundRepository> _logger;

        public RoundRepository(
            BetContext betContext,
            ILogger<RoundRepository> logger)
        {
            _betContext = betContext ?? throw new ArgumentNullException(nameof(betContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> AdjustClosedRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, ClientRoundStatus clientRoundStatus, DateTimeOffset externalClosedOn, Guid operatorId, string remark)
        {
            var data = await _betContext.Rounds
                .Where(x => x.Id == roundId)
                .FirstAsync()
                .ConfigureAwait(false);

            var oldData = data;

            using (var transaction = _betContext.Database.BeginTransaction())
            {
                try
                {
                    var systemDateTimeOffset = DateTime.UtcNow;
                    data.UpdatedOn = systemDateTimeOffset;
                    data.ClosedOn = systemDateTimeOffset;
                    data.ExternalClosedOn = externalClosedOn.UtcDateTime;
                    data.BetAmount = betAmount;
                    data.ValidBetAmount = validBetAmount;
                    data.SettleAmount = settleAmount;
                    data.Commission = commission;
                    data.ReopenTurnoverAdjustment = data.Turnover;
                    data.ReopenedOn = systemDateTimeOffset;
                    data.Turnover = turnover;
                    data.WinLoss = winLoss;
                    data.ClientStatus = (short)clientRoundStatus;
                    _betContext.Update(data);
                    _ = await _betContext.SaveChangesAsync().ConfigureAwait(false);

                    _ = await _betContext.RoundOperationRecords.AddAsync(new()
                    {
                        Id = Guid.NewGuid(),
                        RoundId = roundId,
                        OperatorId = operatorId,
                        OldData = JsonSerializer.Serialize(oldData),
                        NewData = JsonSerializer.Serialize(data),
                        CreatedOn = systemDateTimeOffset,
                        Remark = remark
                    }).ConfigureAwait(false);
                    _ = await _betContext.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                    _logger.LogError($"An exception occurs when adjusting the closed game record.({roundId})", ex);
                    return false;
                }
            }
        }

        public async ValueTask<bool> ClosedGameRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, ClientRoundStatus clientRoundStatus, DateTimeOffset externalClosedOn, Guid operatorId, string remark)
        {
            var data = await _betContext.Rounds
                .Where(x => x.Id == roundId)
                .FirstAsync()
                .ConfigureAwait(false);

            var oldDate = data;

            using (var transaction = _betContext.Database.BeginTransaction())
            {
                try
                {
                    var systemDateTimeOffset = DateTime.UtcNow;
                    data.Status = (short)RoundStatus.Closed;
                    data.UpdatedOn = systemDateTimeOffset;
                    data.ClosedOn = systemDateTimeOffset;
                    data.ExternalClosedOn = externalClosedOn.UtcDateTime;
                    data.BetAmount = betAmount;
                    data.ValidBetAmount = validBetAmount;
                    data.SettleAmount = settleAmount;
                    data.Commission = commission;
                    data.Turnover = turnover;
                    data.WinLoss = winLoss;
                    data.ClientStatus = (short)clientRoundStatus;
                    _betContext.Update(data);
                    _ = await _betContext.SaveChangesAsync().ConfigureAwait(false);

                    _ = await _betContext.RoundOperationRecords.AddAsync(new()
                    {
                        Id = Guid.NewGuid(),
                        RoundId = roundId,
                        OperatorId = operatorId,
                        OldData = JsonSerializer.Serialize(oldDate),
                        NewData = JsonSerializer.Serialize(data),
                        CreatedOn = systemDateTimeOffset,
                        Remark = remark
                    }).ConfigureAwait(false);
                    _ = await _betContext.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                    _logger.LogError($"An exception occurs when closing the game record.({roundId})", ex);
                    return false;
                }
            }
        }

        public IAsyncEnumerable<GameActionModel> GameActionQueryAsync(Guid roundId)
            =>
            _betContext
            .GameActions
            .AsAsyncEnumerable()
            .Where(x => x.RoundId == roundId)
            .Select(x => new GameActionModel(
                 x.Id,
                 x.RoundId,
                 x.PlayerCode,
                 x.ExternalTransactionId,
                 (GameActionType)x.Type,
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

        public async ValueTask<GameRoundModel?> GetAsync(Guid roundId)
            => await _betContext
            .Rounds
            .Where(x => x.Id == roundId)
            .Select(x => new GameRoundModel(
                x.Id,
                x.ExternalRoundId,
                x.GameProviderId,
                x.PlayerCode,
                x.GameId,
                x.CurrencyCode,
                (RoundStatus)x.Status,
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
                (ClientRoundStatus)x.ClientStatus,
                x.ReopenTurnoverAdjustment,
                x.ReopenedOn,
                x.EffectiveBettingPercentage
                ))
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}
