using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class FundInCommandService : ICommandService<FundInCommand>
    {
        private readonly IGameFundTransferClient _client;
        private readonly IGameProvider _gameProvider;
        private readonly IPlayer _player;
        private readonly IPlayerGameBalance _playerGameBalance;

        public FundInCommandService(
            IGameFundTransferClient client,
            IGameProvider gameProvider,
            IPlayer player,
            IPlayerGameBalance playerGameBalance)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
        }

        public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
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

            var gameProviderBalance = await _playerGameBalance.GetBalanceAsync(
                gameProviderCode: gameProvider.GameProviderCode,
                playerCode: player.PlayerCode,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var amount = gameProviderBalance;

            amount *= 100;
            amount = Math.Floor(amount);

            if (amount > 0)
            {
                amount /= 100;
            }

            if (amount == 0)
            {
                return;
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

            _ = await _client.FundInAsync(
                 request: request,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
