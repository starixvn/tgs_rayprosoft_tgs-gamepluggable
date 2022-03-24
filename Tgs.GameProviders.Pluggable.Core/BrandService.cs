using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

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

        public ValueTask<string?> GetCurrencyCodeAsync(Guid brandId, string currencyCode)
            => _gameProvidersQueryServerClient.BrandGetCurrencyAsync(brandId, currencyCode);

        public async ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId)
        {
            var status = await _gameProvidersQueryServerClient.BrandGetGameProviderStatusAsync(brandId, gameProviderId).ConfigureAwait(false);
            return status ?? throw new ArgumentException($"The  game provider status provider doesn't exist.{gameProviderId}");
        }

        public async ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId)
        {
            var status = await _gameProvidersQueryServerClient.BrandGetGameStatusAsync(brandId, gameId).ConfigureAwait(false);
            return status ?? throw new ArgumentException($"The  game status provider doesn't exist.{gameId}");
        }

        public async ValueTask<BrandStatus> GetStatusAsync(Guid brandId)
        {
            var status = await _gameProvidersQueryServerClient.BrandGetStatusAsync(brandId).ConfigureAwait(false);
            return status ?? throw new ArgumentException($"The  brand status provider doesn't exist.{brandId}");
        }
    }
}
