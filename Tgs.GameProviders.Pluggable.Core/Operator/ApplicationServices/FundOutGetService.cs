using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Client;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class FundOutGetService : ICommandService<FundOutQuery>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerificationGrpc _gameVerificationGrpc;
        private readonly IGameFundTransferGrpcService _grpcService;
        private readonly IPlayerService _playerService;

        public FundOutGetService(
            ICashServiceClient cashServiceClient,
            IGameFundTransferGrpcService grpcService,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IGameVerificationGrpc gameVerificationGrpc,
            IPlayerService playerService)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _grpcService = grpcService ?? throw new ArgumentNullException(nameof(grpcService));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationGrpc = gameVerificationGrpc ?? throw new ArgumentNullException(nameof(gameVerificationGrpc));
        }

        public async ValueTask ExecuteAsync(FundOutQuery query, CancellationToken cancellationToken = default)
        {
            var amount = Math.Abs(query.Amount);

            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            if (!await _gameVerificationGrpc.VerifyPlayerFundAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false))
            {
                throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
            }

            _ = await _playerService.GetBalanceAsync(
                    gameProvider.GameProviderCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            var playerBalance = await _cashServiceClient.GetBalanceAsync(player.DefaultWalletId, cancellationToken).ConfigureAwait(false);

            if (playerBalance - amount < 0)
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
                Amount = amount
            };

            _ = await _grpcService.FundOutAsync(
                 request: request,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
