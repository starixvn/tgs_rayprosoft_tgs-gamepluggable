using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class PlayerService : IPlayerService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public PlayerService(IGameProvidersQueryServerClient gameProvidersQueryServerClient)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async ValueTask<PlayerInfo> GetAsync(string playerCode)
        {
            var data = await _gameProvidersQueryServerClient.PlayerGetAsync(playerCode).ConfigureAwait(false);

            return data != null
                ? (new(
                    data.ResellerId,
                    data.ResellerCode,
                    data.LicenseeId,
                    data.LicenseeCode,
                    data.BrandId,
                    data.BrandCode,
                    data.PlayerCode,
                    data.Username,
                    data.DefaultWalletId,
                    data.DefaultWalletCurrencyCode,
                    data.Status))
                : throw new Exception($"This player does not exist.({playerCode})"); ;
        }
    }
}
