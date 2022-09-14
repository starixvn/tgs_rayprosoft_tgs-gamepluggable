using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameVerification.Client;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerificationClient _gameVerificationClient;
        private readonly IPlayerService _playerService;

        public PlayerBalanceGetService(
            IGameProviderService gameProviderService,
            IPlayerService playerService,
            IGameVerificationClient gameVerificationClient)
        {
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(
                query.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(query.GameProviderCode).ConfigureAwait(false);

            return await _gameVerificationClient.VerifyPlayerGameProviderCurrencyAsync(player.BrandId, gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false)
                ? await _playerService.GetBalanceAsync(
                    query.GameProviderCode,
                    query.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false)
                : null;
        }
    }
}
