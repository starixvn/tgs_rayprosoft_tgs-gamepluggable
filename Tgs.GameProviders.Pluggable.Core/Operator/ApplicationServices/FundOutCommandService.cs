using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class FundOutCommandService : ICommandService<FundOutCommand>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameFundTransferClient _gameFundTransferClient;
        private readonly IGameProvider _gameProvider;
        private readonly IGameVerificationClient _gameVerificationClient;
        private readonly IPlayer _player;
        private readonly IPlayerGameBalance _playerGameBalance;

        public FundOutCommandService(
            ICashServiceClient cashServiceClient,
            IGameFundTransferClient gameFundTransferClient,
            IGameProvider gameProvider,
            IGameVerificationClient gameVerificationClient,
            IPlayer player,
            IPlayerGameBalance playerGameBalance)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameFundTransferClient = gameFundTransferClient ?? throw new ArgumentNullException(nameof(gameFundTransferClient));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
        }

        public async ValueTask ExecuteAsync(FundOutCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _player
                            .GetAsync(
                                command.PlayerCode,
                                cancellationToken: cancellationToken)
                            .ConfigureAwait(false);

            var gameProviderId = await _gameProvider
                            .GetGameProviderIdAsync(
                                command.GameProviderCode,
                                cancellationToken)
                            .ConfigureAwait(false);

            var gameProvider = await _gameProvider
                            .GetAsync(
                                gameProviderId,
                                cancellationToken)
                            .ConfigureAwait(false);

            if (!await _gameVerificationClient.VerifyPlayerFundAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false))
            {
                throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
            }

            _ = await _playerGameBalance.GetBalanceAsync(
                    gameProvider.GameProviderCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            var playerBalance = await _cashServiceClient.GetBalanceAsync(player.DefaultWalletId, cancellationToken).ConfigureAwait(false);

            if (playerBalance - command.Amount < 0)
            {
                throw new Exception($"Insufficient balance in main wallet.({playerBalance})");
            }

            var request = new GameFundTransferFundRequest()
            {
                GameProviderId = gameProvider.GameProviderId,
                GameProviderCode = gameProvider.GameProviderCode,
                BrandCode = player.BrandCode,
                PlayerCode = player.PlayerCode,
                CurrencyCode = player.DefaultWalletCurrencyCode,
                WalletId = player.DefaultWalletId,
                Amount = command.Amount
            };

            _ = await _gameFundTransferClient.FundOutAsync(
                 request: request,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
