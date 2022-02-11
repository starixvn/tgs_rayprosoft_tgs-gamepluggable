using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class PlayerVerifyHanlde : IPlayerVerifyHanlde
    {
        private readonly IBrandService _brandService;
        private readonly IGameProviderService _gameProviderService;
        private readonly ILicenseeService _licenseeService;
        private readonly IResellerService _resellerService;

        public PlayerVerifyHanlde(
            IBrandService brandService,
            IGameProviderService gameProviderService,
            ILicenseeService licenseeService,
            IResellerService resellerService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _licenseeService = licenseeService ?? throw new ArgumentNullException(nameof(licenseeService));
            _resellerService = resellerService ?? throw new ArgumentNullException(nameof(resellerService));
        }

        public async ValueTask<bool> IsPlayerEnableAsync(PlayerInfo player)
        {
            var brandStatus = await _brandService.GetStatusAsync(player.BrandId).ConfigureAwait(false);
            var licenseeStatus = await _licenseeService.GetStatusAsync(player.LicenseeId).ConfigureAwait(false);
            var resellerStatus = await _resellerService.GetStatusAsync(player.ResellerId).ConfigureAwait(false);
            return (
                resellerStatus == ResellerStatus.Active
                &&
                licenseeStatus == LicenseeStatus.Active
                &&
                brandStatus == BrandStatus.Active
                &&
                player.Status == PlayerStatus.Enabled
                );
        }

        public async ValueTask<bool> IsPlayerGameActiveAsync(Guid brandId, GameInfo game)
        {
            var gameProvider = await _gameProviderService.GetAsync(game.GameProviderId).ConfigureAwait(false);

            var brandGameProviderStatus = await _brandService.GetGameProviderStatusAsync(brandId, gameProvider.GameProviderId).ConfigureAwait(false);

            var brandGameStatus = await _brandService.GetGameStatusAsync(brandId, game.GameId).ConfigureAwait(false);

            return
                game.Status == GameStatus.Active &&
                gameProvider.Status == GameProviderStatus.Active &&
                brandGameProviderStatus == GameProviderStatus.Active &&
                brandGameStatus == GameStatus.Active;
        }

        public async ValueTask<bool> IsPlayerGameProviderActiveAsync(Guid brandId, GameProviderInfo gameProvider)
        {
            var brandGameProviderStatus = await _brandService.GetGameProviderStatusAsync(brandId, gameProvider.GameProviderId).ConfigureAwait(false);

            return
                gameProvider.Status == GameProviderStatus.Active &&
                brandGameProviderStatus == GameProviderStatus.Active;
        }

        public async ValueTask<bool> IsPlayerGameProviderCurrencyAsync(Guid brandId, Guid gameProviderId, string currencyCode)
        {
            var gameProviderCurrency = await _gameProviderService.GetCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            var brandCurrency = await _brandService.GetCurrencyCodeAsync(brandId, currencyCode).ConfigureAwait(false);

            return !string.IsNullOrEmpty(gameProviderCurrency) && !string.IsNullOrEmpty(brandCurrency);
        }
    }
}
