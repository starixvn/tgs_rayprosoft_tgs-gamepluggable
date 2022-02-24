using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class GameRoundGetService : IGetService<Guid, GameRoundInfo>
    {
        private readonly IRoundRepository _roundRepository;

        public GameRoundGetService(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask<GameRoundInfo> GetAsync(Guid query, CancellationToken cancellationToken = default)
        {
            var data = await _roundRepository
                .GetAsync(query)
                .ConfigureAwait(false);

            return data != null ? data : throw new Exception($"Game Round does not exist.({query})");
        }
    }
}
