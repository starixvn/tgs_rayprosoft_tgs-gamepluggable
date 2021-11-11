using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, PlayerBalanceResult?>
    {
        private readonly ILogger<GameUrlGetService> _logger;
        private readonly IPlayerVerifyService _verifyService;

        public PlayerBalanceGetService(
            IPlayerVerifyService verifyService,
            ILogger<GameUrlGetService> logger)
        {
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<PlayerBalanceResult?> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
        {
            if (!await _verifyService.IsPlayerEnableAsync(query.PlayerCode).ConfigureAwait(false))
            {
                return null;
            }

            return new();
        }
    }
}
