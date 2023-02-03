using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class FundInCommandService : ICommandService<FundInCommand>
{
    private readonly IGameFundTransferClient _gameFundTransferClient;
    private readonly IGameProvider _gameProvider;
    private readonly IGameVerificationClient _gameVerificationClient;
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;

    public FundInCommandService(
      IGameFundTransferClient gameFundTransferClient,
      IGameProvider gameProvider,
      IGameVerificationClient gameVerificationClient,
      IPlayer player,
      IPlayerGameBalance playerGameBalance)
    {
        _gameFundTransferClient = gameFundTransferClient ?? throw new ArgumentNullException(nameof(gameFundTransferClient));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
    }

    public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount == 0)
        {
            return;
        }

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

        if (!gameProvider.TransferWallet)
        {
            throw new Exception($"This game provider does not provide multi-wallet functionality.({gameProvider.GameProviderCode})");
        }

        if (!await _gameVerificationClient.VerifyPlayerFundAsync(player.PlayerCode, gameProvider.GameProviderId).ConfigureAwait(false))
        {
            throw new Exception("Transfer verification failed. (1. The player does not exist or is not enabled. 2. The game manufacturer does not exist or is not enabled. 3. The game market does not support this currency.)");
        }

        var gameProviderBalance = await _playerGameBalance.GetBalanceAsync(
                 gameProvider.GameProviderCode,
                 player.PlayerCode,
                 player.DefaultWalletCurrencyCode,
                 cancellationToken: cancellationToken).ConfigureAwait(false);

        if (gameProviderBalance - command.Amount < 0)
        {
            throw new Exception($"Insufficient game wallet balance.({gameProviderBalance})");
        }

        var amount = command.Amount;

        if (gameProvider.GameProviderCode == GameProviderType.SM01)
            amount = Math.Floor(amount);

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

        _ = await _gameFundTransferClient
            .FundInAsync(
                gameProviderId: gameProvider.GameProviderId,
                gameProviderCode: gameProvider.GameProviderCode,
                brandCode: player.BrandCode,
                playerCode: player.PlayerCode,
                walletId: player.DefaultWalletId,
                currencyCode: player.DefaultWalletCurrencyCode,
                amount: amount,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
