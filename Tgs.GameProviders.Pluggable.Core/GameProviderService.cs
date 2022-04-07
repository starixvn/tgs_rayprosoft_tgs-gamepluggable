using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class GameProviderService : IGameProviderService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;
        private readonly IMemoryCache _memoryCache;

        public GameProviderService(IGameProvidersQueryServerClient gameProvidersQueryServerClient,
            IMemoryCache memoryCache)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId)
        {
            var data = await _gameProvidersQueryServerClient.GameProviderGetInfoAsync(gameProviderId).ConfigureAwait(false);

            return data != null
                ? (new(
                    data.GameProviderId,
                    data.Status,
                    data.Description,
                    data.AutoCloseRound,
                    data.SupportTestPlayer,
                    data.AllowKickPlayer,
                    data.PlaceBetTimeout,
                    data.SettleBetTimeout,
                    data.UniqueId,
                    data.TransferWallet,
                    data.GameProviderCode,
                    data.Name,
                    data.Localization))
                : throw new ArgumentException($"game provider doesn't exist.{gameProviderId}");
        }

        public async ValueTask<GameProviderInfo> GetAsync(string gameProviderCode)
        {
            var gameProviderId = await GetCacheGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            return await GetAsync(gameProviderId).ConfigureAwait(false);
        }

        public async ValueTask<Guid> GetCacheGameProviderIdAsync(string gameProviderCode)
        {
            if (!_memoryCache.TryGetValue(gameProviderCode, out Guid? gameProviderId))
            {
                gameProviderId = await _gameProvidersQueryServerClient.GameProviderGetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
                _ = _memoryCache.Set(gameProviderCode, gameProviderId, TimeSpan.FromDays(1));
            }
            return gameProviderId ?? throw new Exception($"game provider does not exist.({gameProviderCode})");
        }

        public async ValueTask<GameProviderConfigInfo> GetConfigurationAsync(Guid gameProviderConfigId)
        {
            var data = await _gameProvidersQueryServerClient.GameProviderGetGameProviderConfigAsync(gameProviderConfigId).ConfigureAwait(false);

            return data != null
                ? (new(
                data.GameProviderConfigId,
                data.Name,
                data.Description,
                data.ClientId,
                data.SecretKey,
                data.Status,
                data.GameProviderId,
                data.DesktopEndpoint,
                data.MobileEndpoint,
                data.ConfigurationData,
                data.IsDefault))
                : throw new ArgumentException($"game provider config doesn't exist.{gameProviderConfigId}");
        }
    }
}
