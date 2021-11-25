using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class GameUrlGetService : IGetService<GameUrlQuery, Uri?>
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameUrlGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerVerifyService _verifyService;

        public GameUrlGetService(
          IGameProviderRepository gameProviderRepository,
          IGameProvidersClient gameProvidersClient,
          IGameRepository gameRepository,
          ILogger<GameUrlGetService> logger,
          IPlayerRepository playerRepository,
          IPlayerVerifyService verifyService)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
        }

        public async ValueTask<Uri?> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
        {
            if (!await _verifyService.IsPlayerEnableAsync(query.PlayerCode).ConfigureAwait(false))
            {
                return null;
            }

            var playerModel = await _playerRepository.GetAsync(query.PlayerCode);

            if (!await _verifyService.IsPlayerGameActiveAsync(playerModel.BrandId, query.GameId).ConfigureAwait(false))
            {
                return null;
            }
            var gameModel = await _gameRepository.GetAsync(query.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                return null;
            }

            var gameProviderModel = await _gameProviderRepository.GetAsync(gameModel.GameProviderId).ConfigureAwait(false);

            return await _gameProvidersClient.GameUrlAsync(
                gameProviderModel.GameProviderCode,
                gameModel.GameType,
                query.GameId,
                playerModel.BrandCode,
                playerModel.PlayerCode,
                playerModel.DefaultWalletCurrencyCode,
                query.LoginRecordId,
                query.PlatformType,
                query.Language,
                query.IpAddress,
                query.BackUrl)
                .ConfigureAwait(false);
        }
    }
}
