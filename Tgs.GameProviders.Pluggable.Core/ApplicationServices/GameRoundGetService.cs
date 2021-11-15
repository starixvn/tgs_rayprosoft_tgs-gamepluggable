using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class GameRoundGetService : IGetService<Guid, GameRoundModel?>
    {
        private readonly IRoundRepository _roundRepository;

        public GameRoundGetService(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask<GameRoundModel?> GetAsync(Guid query, CancellationToken cancellationToken = default)
            => await _roundRepository.GetAsync(query).ConfigureAwait(false);
    }
}