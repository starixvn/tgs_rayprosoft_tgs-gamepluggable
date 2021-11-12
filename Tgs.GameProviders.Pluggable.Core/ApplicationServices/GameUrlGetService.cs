using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class GameUrlGetService : IGetService<GameUrlQuery, GameUrlResult?>
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ILogger<GameUrlGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerVerifyService _verifyService;

        public GameUrlGetService(
            IPlayerVerifyService verifyService,
            IPlayerRepository playerRepository,
            ILogger<GameUrlGetService> logger,
            IGameProviderRepository gameProviderRepository)
        {
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<GameUrlResult?> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
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

            var subGameProviderModel = await _gameProviderRepository.GetSubGameProvider(query.SubGameProviderCode).ConfigureAwait(false);

            return new();
        }
    }
}
