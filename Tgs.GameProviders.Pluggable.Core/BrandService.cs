using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class BrandService : IBrandService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public BrandService(IGameProvidersQueryServerClient gameProvidersQueryServerClient)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async ValueTask<BrandGameProviderInfo> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId)
        {
            var data = await _gameProvidersQueryServerClient.BrandGetGameProviderConfigurationAsync(brandId, gameProviderId).ConfigureAwait(false);

            return data != null
                ? (new(
                    data.GameProviderId,
                    data.GameProviderConfigurationId,
                    data.DefaultLanguageCode,
                    data.DesktopQueryString,
                    data.MobileQueryString))
                : throw new ArgumentException($"Then brand game provider configuration doesn't exist.({brandId}|{gameProviderId})");
        }
    }
}
