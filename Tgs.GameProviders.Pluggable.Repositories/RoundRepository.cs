using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class RoundRepository : IRoundRepository
    {
        private readonly BetContext _betContext;

        public RoundRepository(BetContext betContext)
        {
            _betContext = betContext ?? throw new ArgumentNullException(nameof(betContext));
        }

        public async ValueTask ClosedGameRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, ClientRoundStatus clientRoundStatus, DateTimeOffset externalClosedOn)
        {
            var data = await _betContext.Rounds
                .Where(x => x.Id == roundId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            var systemDateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).DateTime;
            data.Status = (short)RoundStatus.Closed;
            data.UpdatedOn = systemDateTimeOffset;
            data.ClosedOn = systemDateTimeOffset;
            data.ExternalClosedOn = externalClosedOn.DateTime;
            data.BetAmount = betAmount;
            data.ValidBetAmount = validBetAmount;
            data.SettleAmount = settleAmount;
            data.Commission = commission;
            data.Turnover = turnover;
            data.WinLoss = winLoss * -1;
            data.ClientStatus = (short)clientRoundStatus;
            _betContext.Update(data);
            await _betContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<GameRoundModel> GetAsync(Guid roundId)
            => _betContext
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
            .FirstOrDefaultAsync();
    }
}
