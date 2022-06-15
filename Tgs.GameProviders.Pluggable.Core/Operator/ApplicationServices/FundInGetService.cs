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
    internal class FundInGetService : IGetService<FundInQuery, FundResult?>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameVerificationGrpc _gameVerificationGrpc;
        private readonly ILogger<FundInGetService> _logger;
        private readonly IPlayerService _playerService;

        public FundInGetService(
            ICashServiceClient cashServiceClient,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            IGameVerificationGrpc gameVerificationGrpc,
            ILogger<FundInGetService> logger,
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

        public async ValueTask<FundResult?> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
        {
            var amount = Math.Abs(query.Amount);

            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            if (!await _gameVerificationGrpc.VerifyPlayerFundAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false))
            {
                throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
            }

            var gameProviderBalance = await _gameProvidersClient.PlayerBalanceAsync(
                query.GameProviderCode,
                player.PlayerCode).ConfigureAwait(false);

            if (gameProviderBalance - amount < 0)
            {
                throw new Exception($"Insufficient game wallet balance.({gameProviderBalance})");
            }

            var gameProviderFundTransferTransactionId = await _gameProviderFundTransferTransactionRepository.AddFundInAsync(
                gameProvider.GameProviderId,
                player.DefaultWalletId,
                player.PlayerCode,
                gameProvider.GameProviderCode,
                amount,
                string.Empty)
                .ConfigureAwait(false);

            var gameProviderTransactionId = await _gameProvidersClient.PlayerFundInAsync(
                gameProvider.GameProviderCode,
                player.BrandCode,
                player.PlayerCode,
                player.DefaultWalletCurrencyCode,
                gameProviderFundTransferTransactionId.ToString("N"),
                amount)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(gameProviderTransactionId))
            {
                await _gameProviderFundTransferTransactionRepository.UpdateByFailureOnGameProviderAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    string.Empty,
                    -1,
                    string.Empty
                     )
                    .ConfigureAwait(false);
                throw new Exception("The game manufacturer's transfer failed.");
            }
            TransactionResponse? transactionResponse = default;
            try
            {
                transactionResponse = await _cashServiceClient.GameProviderFundInAsync(
                   player.DefaultWalletId,
                   amount,
                   $"【{gameProvider.GameProviderCode}】->【Default】",
                   gameProviderFundTransferTransactionId.ToString(),
                   cancellationToken)
                   .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                transactionResponse = null;
                _logger.LogError(exception: ex, "The main wallet transfer failed.({gameProviderFundTransferTransactionId})", gameProviderFundTransferTransactionId);
            }
            finally
            {
                if (transactionResponse == null)
                {
                    await _gameProviderFundTransferTransactionRepository.UpdateByFailureOnMainWalletAsync(
                        gameProviderFundTransferTransactionId,
                        null,
                        gameProviderTransactionId,
                        -1,
                        string.Empty
                        )
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
                                    )
                                    .ConfigureAwait(false);
                }
            }

            return (transactionResponse == null) ?
                null :
                new(transactionResponse.TransactionId, gameProviderTransactionId);
        }
    }
}
