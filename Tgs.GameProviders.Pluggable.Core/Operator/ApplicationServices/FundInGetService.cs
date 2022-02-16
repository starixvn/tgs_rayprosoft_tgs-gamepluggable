using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    internal class FundInGetService : IGetService<FundInQuery, FundResult?>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly ILogger<FundInGetService> _logger;
        private readonly IPlayerService _playerService;
        private readonly IPlayerVerifyHanlde _playerVerify;

        public FundInGetService(
            ICashServiceClient cashServiceClient,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService,
            ILogger<FundInGetService> logger,
            IPlayerService playerService,
            IPlayerVerifyHanlde playerVerify
            )
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerVerify = playerVerify ?? throw new ArgumentNullException(nameof(playerVerify));
        }

        public async ValueTask<FundResult?> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
        {
            var amount = Math.Abs(query.Amount);

            var player = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);

            if (!await _playerVerify.IsPlayerEnableAsync(player).ConfigureAwait(false))
            {
                throw new Exception($"The player does not exist or the status is not enabled.({query.PlayerCode})");
            }

            var gameProvider = await _gameProviderService.GetAsync(query.GameProviderCode).ConfigureAwait(false);

            if (!await _playerVerify.IsPlayerGameProviderActiveAsync(player.BrandId, gameProvider).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not exist or the status is not enabled.({query.GameProviderCode})");
            }

            if (!await _playerVerify.IsPlayerGameProviderCurrencyAsync(player.BrandId, gameProvider.GameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false))
            {
                throw new Exception($"The game manufacturer does not support this currency.({player.DefaultWalletCurrencyCode})");
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
                   $"【{ gameProvider.GameProviderCode}】->【Default】",
                   gameProviderFundTransferTransactionId.ToString())
                   .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                transactionResponse = null;
                _logger.LogError($"The main wallet transfer failed.({gameProviderFundTransferTransactionId})", ex);
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
