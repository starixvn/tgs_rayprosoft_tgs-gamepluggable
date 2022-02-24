using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class GameUrlGetService : IGetService<GameUrlQuery, Uri?>
    {
        private readonly IBrandService _brandService;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ILogger<GameUrlGetService> _logger;
        private readonly IPlayerService _playerService;
        private readonly IPlayerVerifyHanlde _verifyService;

        public GameUrlGetService(
            IBrandService brandService,
            ICashServiceClient cashServiceClient,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IGameService gameService,
            ILogger<GameUrlGetService> logger,
            IPlayerService playerService,
            IPlayerVerifyHanlde verifyService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
        }

        public async ValueTask<Uri?> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            if (!await _verifyService.IsPlayerEnableAsync(player).ConfigureAwait(false))
            {
                throw new Exception($"The player does not exist or the status is not enabled.({query.PlayerCode})");
            }

            var game = await _gameService.GetAsync(query.GameId).ConfigureAwait(false);

            if (!await _verifyService.IsPlayerGameActiveAsync(player.BrandId, game).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not exist or the status is not enabled.({query.GameId})");
            }

            if (!await _verifyService.IsPlayerGameProviderCurrencyAsync(player.BrandId, game.GameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not support this currency.({player.DefaultWalletCurrencyCode})");
            }

            var gameProvider = await _gameProviderService.GetAsync(game.GameProviderId).ConfigureAwait(false);

            if (gameProvider.GameProviderCode == "AG")
            {
                return await GetAsiaGamingUrlAsync(
                        query,
                        player,
                        gameProvider,
                        game)
                    .ConfigureAwait(false);
            }

            if (gameProvider.GameProviderCode == "BC")
            {
                return await GetBetConstructUrlAsync(
                        query,
                        player,
                        gameProvider,
                        game)
                    .ConfigureAwait(false);
            }

            return await _gameProvidersClient
                .GameUrlAsync(
                    gameProvider.GameProviderCode,
                    (GameProvidersClientGameType)game.GameType,
                    query.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl)
                .ConfigureAwait(false);
        }

        private async ValueTask<Uri?> GetAsiaGamingUrlAsync(
            GameUrlQuery query,
            PlayerInfo player,
            GameProviderInfo gameProviderInfo,
            GameInfo gameModel)
        {
            var playerBalance = await _cashServiceClient
                .GetBalanceAsync(player.DefaultWalletId)
                .ConfigureAwait(false);

            return await _gameProvidersClient
                .GameUrlByAsiaGamingAsync(
                    gameProviderInfo.GameProviderCode,
                    (GameProvidersClientGameType)gameModel.GameType,
                    query.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl,
                    playerBalance)
                .ConfigureAwait(false);
        }

        private async ValueTask<Uri?> GetBetConstructUrlAsync(
            GameUrlQuery query,
            PlayerInfo player,
            GameProviderInfo gameProviderInfo,
            GameInfo gameModel)
        {
            var brandGameProviderConfiguration = await _brandService
                .GetBrandGameProviderConfigurationAsync(player.BrandId, gameProviderInfo.GameProviderId)
                .ConfigureAwait(false);

            var gameProviderConfiguration = await _gameProviderService
                .GetConfigurationAsync(brandGameProviderConfiguration.GameProviderConfigurationId)
                .ConfigureAwait(false);

            var lobbyDomain = query.PlatformType switch
            {
                GameProvidersClientPlatformType.Desktop => gameProviderConfiguration.DesktopEndpoint,
                GameProvidersClientPlatformType.Mobile => gameProviderConfiguration.MobileEndpoint,
                _ => gameProviderConfiguration.DesktopEndpoint
            };

            return await _gameProvidersClient
                .GameUrlByBetConstructAsync(
                    gameProviderInfo.GameProviderCode,
                    (GameProvidersClientGameType)gameModel.GameType,
                    query.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl,
                    lobbyDomain)
                .ConfigureAwait(false);
        }
    }
}
