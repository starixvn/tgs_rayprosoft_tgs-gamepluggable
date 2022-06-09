using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class GameUrlGetService : IGetService<GameUrlQuery, Uri?>
    {
        private readonly IBrandService _brandService;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;
        private readonly IGameVerificationGrpc _gameVerificationGrpc;
        private readonly IPlayerService _playerService;

        public GameUrlGetService(
            IBrandService brandService,
            ICashServiceClient cashServiceClient,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IGameService gameService,
            IGameVerificationGrpc gameVerificationGrpc,
            IPlayerService playerService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationGrpc = gameVerificationGrpc ?? throw new ArgumentNullException(nameof(gameVerificationGrpc));
        }

        public async ValueTask<Uri?> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            if (!await _gameVerificationGrpc.VerifyPlayerGameUrlAsync(query.PlayerCode, query.GameId).ConfigureAwait(false))
            {
                throw new Exception($"Failed to get game URL. (1. The player does not exist or is not enabled. 2. The game does not exist or is not enabled. 3. The game does not support this currency.)");
            }

            var game = await _gameService.GetAsync(query.GameId).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(game.GameProviderId).ConfigureAwait(false);

            return gameProvider.GameProviderCode switch
            {
                var o when o == GameProviderType.AsiaGaming => await GetAsiaGamingUrlAsync(
                        query,
                        player,
                        gameProvider,
                        game).ConfigureAwait(false),
                var o when o == GameProviderType.BetConstruct => await GetBetConstructUrlAsync(
                        query,
                        player,
                        gameProvider,
                        game).ConfigureAwait(false),
                _ => await _gameProvidersClient.GameUrlAsync(
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
                    query.BackUrl).ConfigureAwait(false)
            };
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
