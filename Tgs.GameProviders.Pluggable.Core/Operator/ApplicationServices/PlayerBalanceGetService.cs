using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameVerification.Client;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IPlayerService _playerService;

        public PlayerBalanceGetService(
            IGameProviderService gameProviderService,
            IPlayerService playerService)
        {
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(
                query.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            return await _playerService.GetBalanceAsync(
                    gameProvider.GameProviderCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false);


        }
    }
}
