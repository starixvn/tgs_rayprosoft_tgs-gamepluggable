using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly ILogger<PlayerBalanceGetService> _logger;
        private readonly IPlayerService _playerService;
        private readonly IPlayerVerifyHanlde _playerVerify;

        public PlayerBalanceGetService(
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            ILogger<PlayerBalanceGetService> logger,
            IPlayerService playerService,
            IPlayerVerifyHanlde playerVerify)
        {
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerVerify = playerVerify ?? throw new ArgumentNullException(nameof(playerVerify));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            if (!await _playerVerify.IsPlayerEnableAsync(player).ConfigureAwait(false))
            {
                throw new Exception($"The player does not exist or the status is not enabled.({query.PlayerCode})");
            }

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            if (!await _playerVerify.IsPlayerGameProviderActiveAsync(player.BrandId, gameProvider).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not exist or the status is not enabled.({query.GameProviderCode})");
            }

            if (!await _playerVerify.IsPlayerGameProviderCurrencyAsync(player.BrandId, gameProvider.GameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not support this currency.({player.DefaultWalletCurrencyCode})");
            }

            return await _gameProvidersClient.PlayerBalanceAsync(
                query.GameProviderCode,
                player.PlayerCode)
                .ConfigureAwait(false);
        }
    }
}
