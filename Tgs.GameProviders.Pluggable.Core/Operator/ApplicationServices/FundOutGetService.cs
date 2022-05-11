using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class FundOutGetService : IGetService<FundOutQuery, FundResult?>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerificationGrpc _gameVerificationGrpc;
        private readonly ILogger<FundOutGetService> _logger;
        private readonly IPlayerService _playerService;

        public FundOutGetService(
            ICashServiceClient cashServiceClient,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IGameVerificationGrpc gameVerificationGrpc,
            ILogger<FundOutGetService> logger,
            IPlayerService playerService)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _gameVerificationGrpc = gameVerificationGrpc ?? throw new ArgumentNullException(nameof(gameVerificationGrpc));
        }

        public async ValueTask<FundResult?> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
        {
            var amount = Math.Abs(query.Amount);

            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            if (!_gameVerificationGrpc.VerifyPlayerFund(player.PlayerCode, gameProvider.GameProviderId, cancellationToken))
            {
                throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
            }

            _ = await _gameProvidersClient.PlayerBalanceAsync(
               query.GameProviderCode,
               player.PlayerCode)
               .ConfigureAwait(false);

            var playerBalance = await _cashServiceClient.GetBalanceAsync(player.DefaultWalletId, cancellationToken).ConfigureAwait(false);

            if (playerBalance - amount < 0)
            {
                throw new Exception($"Insufficient balance in main wallet.({playerBalance})");
            }

            var gameProviderFundTransferTransactionId = await _gameProviderFundTransferTransactionRepository.AddFundOutAsync(
                gameProvider.GameProviderId,
                player.DefaultWalletId,
                player.PlayerCode,
                gameProvider.GameProviderCode,
                amount * -1,
                string.Empty)
                .ConfigureAwait(false);

            var transactionResponse = await _cashServiceClient.GameProviderFundOutAsync(
                player.DefaultWalletId,
                amount,
                $"【Default】->【{ gameProvider.GameProviderCode}】",
                gameProviderFundTransferTransactionId.ToString(),
                cancellationToken)
                .ConfigureAwait(false);

            if (transactionResponse == null)
            {
                await _gameProviderFundTransferTransactionRepository.UpdateByFailureOnMainWalletAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    string.Empty,
                    -1,
                    string.Empty)
                    .ConfigureAwait(false);
                throw new Exception("The main wallet transfer failed.");
            }

            var gameProviderTransactionId = string.Empty;
            try
            {
                gameProviderTransactionId = await _gameProvidersClient.PlayerFundOutAsync(
                   gameProvider.GameProviderCode,
                   player.BrandCode,
                   player.PlayerCode,
                   player.DefaultWalletCurrencyCode,
                   gameProviderFundTransferTransactionId.ToString("N"),
                   amount)
                   .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, "The game manufacturer's transfer failed..({gameProviderFundTransferTransactionId})", gameProviderFundTransferTransactionId);
            }
            finally
            {
                if (string.IsNullOrEmpty(gameProviderTransactionId))
                {
                    await _gameProviderFundTransferTransactionRepository.UpdateByFailureOnGameProviderAsync(
                        gameProviderFundTransferTransactionId,
                        null,
                        string.Empty,
                        -1,
                        string.Empty)
                        .ConfigureAwait(false);
                }
                else
                {
                    await _gameProviderFundTransferTransactionRepository.UpdateBySuccessAsync(
                    gameProviderFundTransferTransactionId,
                    transactionResponse.TransactionId,
                    gameProviderTransactionId,
                    transactionResponse.Balance,
                    string.Empty
                    ).ConfigureAwait(false);
                }
            }

            return (string.IsNullOrEmpty(gameProviderTransactionId)) ?
                null :
                new(transactionResponse.TransactionId, gameProviderTransactionId);
        }
    }
}
