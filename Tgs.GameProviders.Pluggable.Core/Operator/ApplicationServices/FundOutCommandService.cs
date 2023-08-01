using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class FundOutCommandService : ICommandService<FundOutCommand>
{
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IGameFundTransferClient _gameFundTransferClient;
    private readonly IGameProvider _gameProvider;
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;
    private readonly IVerification _verification;

    public FundOutCommandService(
        ICashServiceClient cashServiceClient,
        IGameFundTransferClient gameFundTransferClient,
        IGameProvider gameProvider,
        IVerification verification,
        IPlayer player,
        IPlayerGameBalance playerGameBalance)
    {
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        _gameFundTransferClient = gameFundTransferClient ?? throw new ArgumentNullException(nameof(gameFundTransferClient));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _verification = verification ?? throw new ArgumentNullException(nameof(verification));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
    }

    public async ValueTask ExecuteAsync(FundOutCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount == 0)
        {
            return;
        }

        var player = await _player
            .GetAsync(
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: command.GameProviderCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProvider = await _gameProvider
            .GetAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!gameProvider.TransferWallet)
        {
            throw new Exception($"This game provider does not provide multi-wallet functionality.({gameProvider.GameProviderCode})");
        }

        await _verification
            .VerifyPlayerAsync(
            player: player,
            gameProvider: gameProvider,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = await _playerGameBalance
            .GetBalanceAsync(
            gameProvider.GameProviderCode,
            player.PlayerCode,
            player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerBalance = await _cashServiceClient
            .GetBalanceAsync(
            walletId: player.DefaultWalletId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (playerBalance - command.Amount < 0)
        {
            throw new Exception($"Insufficient balance in main wallet.({playerBalance})");
        }

        var amount = command.Amount;

        if (gameProvider.GameProviderCode == GameProviderType.SM01)
            amount = Math.Floor(command.Amount);

        _ = await _gameFundTransferClient
            .FundOutAsync(
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
