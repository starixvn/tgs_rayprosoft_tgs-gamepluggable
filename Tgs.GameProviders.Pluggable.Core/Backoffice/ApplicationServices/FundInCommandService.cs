using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class FundInCommandService : ICommandService<FundInCommand>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameFundTransferGrpcService _grpcService;
        private readonly IPlayerService _playerService;

        public FundInCommandService(
            IGameFundTransferGrpcService grpcService,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IPlayerService playerService)
        {
            _grpcService = grpcService ?? throw new ArgumentNullException(nameof(grpcService));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask ExecuteAsync(FundInCommand query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            var gameProviderBalance = await _gameProvidersClient.PlayerBalanceAsync(
                gameProviderCode: query.GameProviderCode,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var amount = gameProviderBalance.GetValueOrDefault(0);

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

            _ = await _grpcService.FundInAsync(
                 request: request,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
