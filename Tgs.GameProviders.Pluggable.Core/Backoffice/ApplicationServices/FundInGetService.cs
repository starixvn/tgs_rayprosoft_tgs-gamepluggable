using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class FundInGetService : IGetService<FundInQuery, bool>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly ILogger<FundInGetService> _logger;
        private readonly IPlayerService _playerService;

        public FundInGetService(
            ICashServiceClient cashServiceClient,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            ILogger<FundInGetService> logger,
            IPlayerService playerService)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<bool> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            var gameProviderBalance = await _gameProvidersClient.PlayerBalanceAsync(
                query.GameProviderCode,
                player.PlayerCode).ConfigureAwait(false);

            var amount = gameProviderBalance.GetValueOrDefault(0);

            amount *= 100;
            amount = Math.Floor(amount);

            if (amount > 0)
            {
                amount /= 100;
            }

            if (amount == 0)
            {
                return true;
            }

            var gameProviderFundTransferTransactionId = await _gameProviderFundTransferTransactionRepository.AddFundInAsync(
                gameProvider.GameProviderId,
                player.DefaultWalletId,
                player.PlayerCode,
                gameProvider.GameProviderCode,
                amount,
                string.Empty).ConfigureAwait(false);

            var gameProviderTransactionId = await _gameProvidersClient.PlayerFundInAsync(
                gameProvider.GameProviderCode,
                player.BrandCode,
                player.PlayerCode,
                player.DefaultWalletCurrencyCode,
                gameProviderFundTransferTransactionId.ToString("N"),
                amount).ConfigureAwait(false);

            if (string.IsNullOrEmpty(gameProviderTransactionId))
            {
                await _gameProviderFundTransferTransactionRepository.UpdateByFailureOnGameProviderAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    string.Empty,
                    -1,
                    string.Empty
                     ).ConfigureAwait(false);
                throw new Exception("The game manufacturer's transfer failed.");
            }

            try
            {
                var transactionResponse = await _cashServiceClient.GameProviderFundInAsync(
                    player.DefaultWalletId,
                    amount,
                    $"【{gameProvider.GameProviderCode}】->【Default】",
                    gameProviderFundTransferTransactionId.ToString(),
                    cancellationToken).ConfigureAwait(false);

                await _gameProviderFundTransferTransactionRepository.UpdateBySuccessAsync(
                                gameProviderFundTransferTransactionId,
                                transactionResponse.TransactionId,
                                gameProviderTransactionId,
                                transactionResponse.Balance,
                                string.Empty
                                )
                                .ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, "The main wallet transfer failed.({gameProviderFundTransferTransactionId})", gameProviderFundTransferTransactionId);

                await _gameProviderFundTransferTransactionRepository.UpdateByFailureOnMainWalletAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    gameProviderTransactionId,
                    -1,
                    string.Empty
                    )
                    .ConfigureAwait(false);

                throw new Exception($"The main wallet transfer failed.({gameProviderFundTransferTransactionId})");
            }
        }
    }
}
