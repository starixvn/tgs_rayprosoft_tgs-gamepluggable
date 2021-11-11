using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Services
{
    internal class PlayerVerifyService : IPlayerVerifyService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILicenseeRepository _licenseeRepository;
        private readonly ILogger<PlayerVerifyService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IResellerRepository _resellerRepository;

        public PlayerVerifyService(
            IBrandRepository brandRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            ILicenseeRepository licenseeRepository,
            IPlayerRepository playerRepository,
            ILogger<PlayerVerifyService> logger,
            IResellerRepository resellerRepository
            )
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _licenseeRepository = licenseeRepository ?? throw new ArgumentNullException(nameof(licenseeRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resellerRepository = resellerRepository ?? throw new ArgumentNullException(nameof(resellerRepository));
        }

        public async ValueTask<bool> IsPlayerEnableAsync(string playerCode)
        {
            var playerModel = await _playerRepository.GetAsync(playerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                return false;
            }
            var brandStatus = await _brandRepository.GetStatusAsync(playerModel.BrandId).ConfigureAwait(false);
            var licenseeStatus = await _licenseeRepository.GetStatusAsync(playerModel.LicenseeId).ConfigureAwait(false);
            var resellerStatus = await _resellerRepository.GetStatusAsync(playerModel.ResellerId).ConfigureAwait(false);

            return (
                resellerStatus == ResellerStatus.Active
                &&
                licenseeStatus == LicenseeStatus.Active
                &&
                brandStatus == BrandStatus.Active
                &&
                playerModel.Status == PlayerStatus.Enabled
                );
        }

        public async ValueTask<bool> IsPlayerGameActiveAsync(Guid brandId, int gameId)
        {
            var gameModel = await _gameRepository.GetAsync(gameId).ConfigureAwait(false);
            var gameProviderModel = await _gameProviderRepository.GetAsync(gameModel.GameProviderId).ConfigureAwait(false);
            var brandGameProviderStatus = await _brandRepository.GetGameProviderStatusAsync(brandId, gameProviderModel.GameProviderId).ConfigureAwait(false);
            var brandGameStatus = await _brandRepository.GetGameStatusAsync(brandId, gameModel.GameId).ConfigureAwait(false);
            return
                gameModel.Status == GameStatus.Active &&
                gameProviderModel.Status == GameProviderStatus.Active &&
                brandGameProviderStatus == GameProviderStatus.Active &&
                brandGameStatus == GameStatus.Active;
        }

        public async ValueTask<bool> IsPlayerGameProviderActiveAsync(Guid brandId, string gameProviderCode)
        {
            var gameProviderModel = await _gameProviderRepository.GetAsync(gameProviderCode).ConfigureAwait(false);
            var brandGameProviderStatus = await _brandRepository.GetGameProviderStatusAsync(brandId, gameProviderModel.GameProviderId).ConfigureAwait(false);
            return
                gameProviderModel.Status == GameProviderStatus.Active &&
                brandGameProviderStatus == GameProviderStatus.Active;
        }
    }
}
