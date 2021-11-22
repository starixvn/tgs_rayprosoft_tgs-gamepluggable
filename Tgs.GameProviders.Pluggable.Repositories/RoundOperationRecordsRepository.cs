using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class RoundOperationRecordsRepository : IRoundOperationRecordsRepository
    {
        private readonly BetContext _betContext;

        public RoundOperationRecordsRepository(BetContext betContext)
        {
            _betContext = betContext ?? throw new ArgumentNullException(nameof(betContext));
        }

        public async ValueTask AddAsync(Guid operatorId, Guid roundId, string oldData, string newData, string remark)
        {
            await _betContext.RoundOperationRecords.AddAsync(new()
            {
                Id = Guid.NewGuid(),
                RoundId = roundId,
                OperatorId = operatorId,
                OldData = oldData,
                NewData = newData,
                CreatedOn = DateTime.UtcNow,
                Remark = remark
            }).ConfigureAwait(false);
            await _betContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
