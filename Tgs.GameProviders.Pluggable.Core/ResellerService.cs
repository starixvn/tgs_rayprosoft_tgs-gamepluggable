using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class ResellerService : IResellerService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public ResellerService(IGameProvidersQueryServerClient gameProvidersQueryServerClient)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async ValueTask<ResellerStatus> GetStatusAsync(Guid resellerId)
        {
            var status = await _gameProvidersQueryServerClient.ResellerGetStatusAsync(resellerId).ConfigureAwait(false);
            return status.HasValue ? status.Value : throw new ArgumentException($"The reseller status provider doesn't exist.{resellerId}");
        }
    }
}
