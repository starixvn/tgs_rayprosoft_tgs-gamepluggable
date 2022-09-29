using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameVerification.Client;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal>
    {
        private readonly IGameProvider _gameProvider;
        private readonly IGameVerificationClient _gameVerificationClient;
        private readonly IPlayer _player;
        private readonly IPlayerGameBalance _playerGameBalance;

        public PlayerBalanceGetService(
            IGameProvider gameProvider,
            IPlayer player,
            IGameVerificationClient gameVerificationClient,
             IPlayerGameBalance playerGameBalance)
        {
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
            _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
        }

        public async ValueTask<decimal> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _player.GetAsync(
                query.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            var gameProviderId = await _gameProvider
                                    .GetGameProviderIdAsync(
                                        query.GameProviderCode,
                                        cancellationToken)
                                    .ConfigureAwait(false);

            return await _gameVerificationClient.VerifyPlayerGameProviderCurrencyAsync(player.BrandId, gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false)
                ? await _playerGameBalance.GetBalanceAsync(
                    query.GameProviderCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false)
                : throw new Exception($"Player game balance verification failed.({player.PlayerCode})");
        }
    }
}
