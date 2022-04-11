using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameVerifys.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerifysClient _gameVerifysClient;
        private readonly IPlayerService _playerService;

        public PlayerBalanceGetService(
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IPlayerService playerService,
            IGameVerifysClient gameVerifysClient)
        {
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerifysClient = gameVerifysClient ?? throw new ArgumentNullException(nameof(gameVerifysClient));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            return await _gameVerifysClient.VerifyPlayerBalanceAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false)
                ? await _gameProvidersClient.PlayerBalanceAsync(
                    gameProvider.GameProviderCode,
                    player.PlayerCode).ConfigureAwait(false)
                : throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
        }
    }
}
