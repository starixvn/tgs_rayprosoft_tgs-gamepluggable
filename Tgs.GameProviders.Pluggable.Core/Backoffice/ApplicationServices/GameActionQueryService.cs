using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class GameActionQueryService : IQueryService<Guid, GameActionModel>
    {
        private readonly IRoundRepository _roundRepository;

        public GameActionQueryService(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public IAsyncEnumerable<GameActionModel> QueryAsync(Guid query, CancellationToken cancellationToken = default)
            => _roundRepository.GameActionQueryAsync(query);
    }
}
