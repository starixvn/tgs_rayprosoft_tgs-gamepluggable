using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

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
        private readonly IBrandRepository _brandRepository;

        public GameUrlGetService(
          IGameProviderRepository gameProviderRepository,
          IGameProvidersClient gameProvidersClient,
          IGameRepository gameRepository,
          ILogger<GameUrlGetService> logger,
          IPlayerRepository playerRepository,
          IPlayerVerifyService verifyService,
          IBrandRepository brandRepository)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public async ValueTask<Uri?> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
        {
            if (!await _verifyService.IsPlayerEnableAsync(query.PlayerCode).ConfigureAwait(false))
            {
                throw new Exception($"The player does not exist or the status is not enabled.({query.PlayerCode})");
            }

            var playerModel = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);

            if (!await _verifyService.IsPlayerGameActiveAsync(playerModel.BrandId, query.GameId).ConfigureAwait(false))
            {
                return null;
            }

            var gameModel = await _gameRepository.GetAsync(query.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                return null;
            }

            if (!await _verifyService.IsPlayerGameProviderCurrencyAsync(playerModel.BrandId, gameModel.GameProviderId, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not support this currency.({playerModel.DefaultWalletCurrencyCode})");
            }

            var gameProviderModel = await _gameProviderRepository.GetAsync(gameModel.GameProviderId).ConfigureAwait(false);

            if (gameProviderModel.GameProviderCode == "BC")
            {
                var brandGameProviderConfiguration = await _brandRepository
                    .GetBrandGameProviderConfigurationAsync(playerModel.BrandId, gameProviderModel.GameProviderId)
                    .ConfigureAwait(false);

                var gameProviderConfiguration = await _gameProviderRepository
                    .GetGameProviderConfigurationAsync(gameModel.GameProviderId, brandGameProviderConfiguration.GameProviderConfigurationId)
                    .ConfigureAwait(false);

                var lobbyDomain = query.PlatformType switch
                {
                    PlatformType.Desktop => gameProviderConfiguration.DesktopEndpoint,
                    PlatformType.Mobile => gameProviderConfiguration.MobileEndpoint,
                    _ => gameProviderConfiguration.DesktopEndpoint
                };

                return await _gameProvidersClient
                    .GameBetConstructUrlAsync(
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
                        query.BackUrl,
                        lobbyDomain)
                    .ConfigureAwait(false);
            }
            else
            {
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
}