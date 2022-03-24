using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class LicenseeService : ILicenseeService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public LicenseeService(IGameProvidersQueryServerClient gameProvidersQueryServerClient)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async ValueTask<LicenseeStatus> GetStatusAsync(Guid licenseeId)
        {
            var status = await _gameProvidersQueryServerClient.LicenseeGetStatusAsync(licenseeId).ConfigureAwait(false);
            return status ?? throw new ArgumentException($"The licensee status provider doesn't exist.{licenseeId}");
        }
    }
}
