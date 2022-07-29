using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCurrency.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class PlayerService : IPlayerService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameCurrencyClient _gameCurrencyClient;

        public PlayerService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient,
            IGameProvidersClient gameProvidersClient,
            IGameCurrencyClient gameCurrencyClient)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameCurrencyClient = gameCurrencyClient ?? throw new ArgumentNullException(nameof(gameCurrencyClient));
        }

        public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
        {
            var data = await _gameProvidersQueryServerClient.PlayerGetAsync(
                    playerCode,
                    cancellationToken).ConfigureAwait(false);

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

        public async ValueTask<decimal?> GetBalanceAsync(
            string gameProviderCode,
            string playerCode,
            string currencyCode,
            CancellationToken cancellationToken = default)
        {
            var gameProviderBalance = await _gameProvidersClient.PlayerBalanceAsync(
               gameProviderCode,
               playerCode,
               cancellationToken: cancellationToken).ConfigureAwait(false);

            if (gameProviderBalance.HasValue)
            {
                var exchangeRateBalanceResult = await _gameCurrencyClient.GetExchangeRateBalanceAsync(
                    gameProviderCode,
                    currencyCode,
                    gameProviderBalance.Value,
                    cancellationToken).ConfigureAwait(false);

                return exchangeRateBalanceResult.Balance;
            }
            else
            {
                return null;
            }
        }
    }
}
