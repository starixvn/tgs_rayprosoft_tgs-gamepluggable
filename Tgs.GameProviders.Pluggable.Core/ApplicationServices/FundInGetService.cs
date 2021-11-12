using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class FundInGetService : IGetService<FundInQuery, FundResult?>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly ILogger<FundInGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerVerifyService _verifyService;

        public FundInGetService(
            ICashServiceClient cashServiceClient,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IGameProviderRepository gameProviderRepository,
            IGameProvidersClient gameProvidersClient,
            ILogger<FundInGetService> logger,
            IPlayerRepository playerRepository,
            IPlayerVerifyService verifyService
            )
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
        }

        public async ValueTask<FundResult?> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
        {
            var amount = Math.Abs(query.Amount);

            if (!await _verifyService.IsPlayerEnableAsync(query.PlayerCode).ConfigureAwait(false))
            {
                return null;
            }

            var playerModel = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);

            if (!await _verifyService.IsPlayerGameProviderActiveAsync(playerModel.BrandId, query.GameProviderCode).ConfigureAwait(false))
            {
                return null;
            }

            var gameProviderModel = await _gameProviderRepository.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            var gameProviderBalance = await _gameProvidersClient.PlayerBalanceAsync(query.GameProviderCode, playerModel.BrandCode, playerModel.PlayerCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

            if (!gameProviderBalance.HasValue)
            {
                throw new Exception("Game Provider failed to withdraw balance");
            }

            if (gameProviderBalance - amount < 0)
            {
                throw new Exception("Game Provider balance is insufficient");
            }

            var gameProviderFundTransferTransactionId = await _gameProviderFundTransferTransactionRepository.AddFundInAsync(
                gameProviderModel.GameProviderId,
                playerModel.DefaultWalletId,
                playerModel.PlayerCode,
                gameProviderModel.GameProviderCode,
                amount,
                string.Empty)
                .ConfigureAwait(false);

            var gameProviderTransactionId = await _gameProvidersClient.PlayerFundInAsync(
                gameProviderModel.GameProviderCode,
                playerModel.BrandCode,
                playerModel.PlayerCode,
                playerModel.DefaultWalletCurrencyCode,
                gameProviderFundTransferTransactionId.ToString("N"),
                amount)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(gameProviderTransactionId))
            {
                await _gameProviderFundTransferTransactionRepository.UpdateFundInAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    FundTransferTransactionStatus.FailureOnGameProvider,
                    string.Empty,
                    -1,
                    string.Empty
                     )
                    .ConfigureAwait(false);
                return null;
            }

            var transactionViewModel = await _cashServiceClient.GameProviderFundInAsync(
                playerModel.BrandId,
                playerModel.DefaultWalletId,
                playerModel.PlayerCode,
                amount,
                $"【{ gameProviderModel.GameProviderCode}】->【Default】",
                gameProviderFundTransferTransactionId.ToString())
                .ConfigureAwait(false);

            if (transactionViewModel == null)
            {
                await _gameProviderFundTransferTransactionRepository.UpdateFundInAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    FundTransferTransactionStatus.FailureOnSystem,
                    gameProviderTransactionId,
                    -1,
                    string.Empty
                    )
                    .ConfigureAwait(false);
                return null;
            }

            await _gameProviderFundTransferTransactionRepository.UpdateFundInAsync(
                gameProviderFundTransferTransactionId,
                transactionViewModel.TransactionId,
                FundTransferTransactionStatus.Success,
                gameProviderTransactionId,
                transactionViewModel.Balance,
                string.Empty
                )
                .ConfigureAwait(false);

            return new()
            {
                TransactionsId = transactionViewModel.TransactionId,
                GameProviderTransactionId = gameProviderTransactionId
            };
        }
    }
}
