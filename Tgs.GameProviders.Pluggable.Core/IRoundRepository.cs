using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IRoundRepository
    {
        IAsyncEnumerable<GameActionInfo> GameActionQueryAsync(Guid roundId);

        ValueTask<GameRoundInfo?> GetAsync(Guid roundId);
    }
}
