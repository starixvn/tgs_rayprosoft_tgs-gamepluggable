using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class GameActionQueryService : IQueryService<Guid, GameActionInfo>
    {
        private readonly IRoundRepository _roundRepository;

        public GameActionQueryService(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public IAsyncEnumerable<GameActionInfo> QueryAsync(Guid query, CancellationToken cancellationToken = default)
            => _roundRepository.GameActionQueryAsync(query);
    }
}
