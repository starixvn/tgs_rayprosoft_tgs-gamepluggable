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
        private readonly ILogger<PlayerBalanceGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerVerifyService _playerVerifyService;

        public PlayerBalanceGetService(
            IGameProvidersClient gameProvidersClient,
            ILogger<PlayerBalanceGetService> logger,
            IPlayerRepository playerRepository,
            IPlayerVerifyService playerVerifyService)
        {
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _playerVerifyService = playerVerifyService ?? throw new ArgumentNullException(nameof(playerVerifyService));
        }

        public async ValueTask<decimal?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var playerModel = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);

                if (!await _playerVerifyService.IsPlayerGameProviderCurrencyAsync(playerModel.BrandId, query.GameProviderCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false))
                {
                    throw new Exception($"The game manufacturer does not support this currency.({playerModel.DefaultWalletCurrencyCode})");
                }

                return await _gameProvidersClient.PlayerBalanceAsync(
                    query.GameProviderCode,
                    query.PlayerCode)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.Error,
                    new EventId(1000, nameof(PlayerBalanceGetService)),
                    new
                    {
                        GameProviderCode = query.GameProviderCode,
                        PlayerCode = query.PlayerCode,
                        SendDateTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).ToString("O")
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
