using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
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
