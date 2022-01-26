using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IRoundRepository
    {
        ValueTask<bool> ClosedGameRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, ClientRoundStatus clientRoundStatus, DateTimeOffset externalClosedOn, Guid operatorId, string remark);

        ValueTask<bool> AdjustClosedRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, ClientRoundStatus clientRoundStatus, DateTimeOffset externalClosedOn, Guid operatorId, string remark);

        IAsyncEnumerable<GameActionModel> GameActionQueryAsync(Guid roundId);

        ValueTask<GameRoundModel?> GetAsync(Guid roundId);
    }
}
