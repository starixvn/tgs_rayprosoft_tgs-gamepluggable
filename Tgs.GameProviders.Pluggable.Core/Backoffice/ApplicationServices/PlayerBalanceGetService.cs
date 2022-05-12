using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameVerification.Client;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerificationGrpc _gameVerificationGrpc;
        private readonly IPlayerService _playerService;

        public PlayerBalanceGetService(
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IPlayerService playerService,
            IGameVerificationGrpc gameVerificationGrpc)
        {
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationGrpc = gameVerificationGrpc ?? throw new ArgumentNullException(nameof(gameVerificationGrpc));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(query.GameProviderCode).ConfigureAwait(false);

            return await _gameVerificationGrpc.VerifyPlayerGameProviderCurrencyAsync(player.BrandId, gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false)
                ? await _gameProvidersClient.PlayerBalanceAsync(
                    query.GameProviderCode,
                    query.PlayerCode).ConfigureAwait(false)
                : null;
        }
    }
}
