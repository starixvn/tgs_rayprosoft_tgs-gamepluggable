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
        private readonly IGameVerificationGrpc _gameVerificationGrpc;
        private readonly IPlayerService _playerService;

        public PlayerBalanceGetService(
            IGameProviderService gameProviderService,
            IGameVerificationGrpc gameVerificationGrpc,
            IPlayerService playerService)
        {
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationGrpc = gameVerificationGrpc ?? throw new ArgumentNullException(nameof(gameVerificationGrpc));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(
                query.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            return (await _gameVerificationGrpc.VerifyPlayerBalanceAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false))
                ? await _playerService.GetBalanceAsync(
                    gameProvider.GameProviderCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false)
                : throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
        }
    }
}
