using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal?>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly ILogger<PlayerBalanceGetService> _logger;
        private readonly IPlayerService _playerService;
        private readonly IPlayerVerifyHanlde _playerVerifyService;

        public PlayerBalanceGetService(
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            ILogger<PlayerBalanceGetService> logger,
            IPlayerService playerService,
            IPlayerVerifyHanlde playerVerifyService)
        {
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerVerifyService = playerVerifyService;
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

                var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(query.GameProviderCode).ConfigureAwait(false);

                return !await _playerVerifyService.IsPlayerGameProviderCurrencyAsync(player.BrandId, gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false)
                    ? throw new Exception($"The game manufacturer does not support this currency.({player.DefaultWalletCurrencyCode})")
                    : await _gameProvidersClient.PlayerBalanceAsync(
                        query.GameProviderCode,
                        query.PlayerCode).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.Error,
                    new EventId(1000, nameof(PlayerBalanceGetService)),
                    new
                    {
                        query.GameProviderCode,
                        query.PlayerCode,
                        LogOn = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).ToString("O")
                    },
                    ex,
                    (state, ex) =>
                    $"【Request】->{JsonSerializer.Serialize(state)}" +
                    $"【Exception】->{ex?.ToString()}"
                );
                return null;
            }
        }
    }
}
