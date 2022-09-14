using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class FundInGetService : ICommandService<FundInQuery>
    {
        private readonly IGameFundTransferClient _gameFundTransferClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerificationClient _gameVerificationClient;
        private readonly IPlayerService _playerService;

        public FundInGetService(
            IGameFundTransferClient gameFundTransferClient,
            IGameProviderService gameProviderService,
            IGameVerificationClient gameVerificationClient,
            IPlayerService playerService)
        {
            _gameFundTransferClient = gameFundTransferClient ?? throw new ArgumentNullException(nameof(gameFundTransferClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
        }

        public async ValueTask ExecuteAsync(FundInQuery query, CancellationToken cancellationToken = default)
        {
            var amount = Math.Abs(query.Amount);

            var player = await _playerService.GetAsync(
                query.PlayerCode,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            if (!await _gameVerificationClient.VerifyPlayerFundAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false))
            {
                throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
            }

            var gameProviderBalance = await _playerService.GetBalanceAsync(
                     gameProvider.GameProviderCode,
                     player.PlayerCode,
                     player.DefaultWalletCurrencyCode,
                     cancellationToken: cancellationToken).ConfigureAwait(false);

            if (gameProviderBalance - amount < 0)
            {
                throw new Exception($"Insufficient game wallet balance.({gameProviderBalance})");
            }

            var request = new GameFundTransferFundRequest()
            {
                GameProviderId = gameProvider.GameProviderId,
                GameProviderCode = gameProvider.GameProviderCode,
                BrandCode = player.BrandCode,
                PlayerCode = player.PlayerCode,
                CurrencyCode = player.DefaultWalletCurrencyCode,
                WalletId = player.DefaultWalletId,
                Amount = amount
            };

            _ = await _gameFundTransferClient.FundInAsync(
                 request: request,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
