using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IRoundRepository
    {
        ValueTask<bool> AdjustClosedRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, GameRoundClientStatus clientRoundStatus, DateTimeOffset externalClosedOn, Guid operatorId, string remark);

        ValueTask<bool> ClosedGameRoundAsync(Guid roundId, decimal betAmount, decimal validBetAmount, decimal settleAmount, decimal commission, decimal turnover, decimal winLoss, GameRoundClientStatus clientRoundStatus, DateTimeOffset externalClosedOn, Guid operatorId, string remark);

        IAsyncEnumerable<GameActionInfo> GameActionQueryAsync(Guid roundId);

        ValueTask<GameRoundInfo?> GetAsync(Guid roundId);
    }
}
