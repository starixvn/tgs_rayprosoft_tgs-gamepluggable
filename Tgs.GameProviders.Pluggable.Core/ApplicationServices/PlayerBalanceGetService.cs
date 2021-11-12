using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly ILogger<PlayerBalanceGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerVerifyService _verifyService;

        public PlayerBalanceGetService(
            IGameProvidersClient gameProvidersClient,
            ILogger<PlayerBalanceGetService> logger,
            IPlayerRepository playerRepository,
            IPlayerVerifyService verifyService)
        {
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            if (!await _verifyService.IsPlayerEnableAsync(query.PlayerCode).ConfigureAwait(false))
            {
                return null;
            }

            var playerModel = await _playerRepository.GetAsync(
                query.PlayerCode)
                .ConfigureAwait(false);

            return await _gameProvidersClient.PlayerBalanceAsync(
                query.GameProviderCode,
                playerModel.BrandCode,
                playerModel.PlayerCode,
                playerModel.DefaultWalletCurrencyCode)
                .ConfigureAwait(false);
        }
    }
}
