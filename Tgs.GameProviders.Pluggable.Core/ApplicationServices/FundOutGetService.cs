using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class FundOutGetService : IGetService<FundOutQuery, FundResult?>
    {
        private ICashServiceClient _cashServiceClient;
        private IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private IGameProviderRepository _gameProviderRepository;
        private IGameProvidersClient _gameProvidersClient;
        private IPlayerRepository _playerRepository;
        private IPlayerVerifyService _verifyService;

        public FundOutGetService(
            IPlayerVerifyService verifyService,
            ICashServiceClient cashServiceClient,
            IGameProvidersClient gameProvidersClient,
            IPlayerRepository playerRepository,
            IGameProviderRepository gameProviderRepository,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository
            )
        {
            _verifyService = verifyService ?? throw new ArgumentNullException(nameof(verifyService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
        }

        public async ValueTask<FundResult?> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
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
                new Exception("Game Provider failed to withdraw balance");
            }

            var playerBalance = await _cashServiceClient.BalanceAsync(playerModel.DefaultWalletId).ConfigureAwait(false);

            if (playerBalance - amount < 0)
            {
                new Exception("Game Provider balance is insufficient");
            }

            var gameProviderFundTransferTransactionId = await _gameProviderFundTransferTransactionRepository.AddFundOutAsync(
            gameProviderModel.GameProviderId,
            playerModel.DefaultWalletId,
            playerModel.PlayerCode,
            gameProviderModel.GameProviderCode,
            amount * -1,
            string.Empty)
            .ConfigureAwait(false);

            var transactionViewModel = await _cashServiceClient.GameProviderFundOutAsync(
               playerModel.BrandId,
               playerModel.DefaultWalletId,
               playerModel.PlayerCode,
               amount,
               $"【Default】->【{ gameProviderModel.GameProviderCode}】",
               gameProviderFundTransferTransactionId.ToString()
               )
               .ConfigureAwait(false);

            if (transactionViewModel == null)
            {
                await _gameProviderFundTransferTransactionRepository.UpdateFundOutAsync(
                    gameProviderFundTransferTransactionId,
                    null,
                    FundTransferTransactionStatus.FailureOnSystem,
                    string.Empty,
                    -1,
                    string.Empty
                    )
                    .ConfigureAwait(false);
                return null;
            }

            var gameProviderTransactionId = await _gameProvidersClient.PlayerFundOutAsync(
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

            await _gameProviderFundTransferTransactionRepository.UpdateFundInAsync(
                    gameProviderFundTransferTransactionId,
                    transactionViewModel.TransactionId,
                    FundTransferTransactionStatus.Success,
                    gameProviderTransactionId,
                    transactionViewModel.Balance,
                    string.Empty
                    ).ConfigureAwait(false);

            return new()
            {
                TransactionsId = transactionViewModel.TransactionId,
                GameProviderTransactionId = gameProviderTransactionId
            };
        }
    }
}
