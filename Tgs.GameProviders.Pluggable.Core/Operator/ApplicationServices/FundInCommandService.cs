using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class FundInCommandService : ICommandService<FundInCommand>
{
    private readonly IGameFundTransferClient _gameFundTransferClient;
    private readonly IGameProvider _gameProvider;
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;
    private readonly IVerification _verification;

    public FundInCommandService(
      IGameFundTransferClient gameFundTransferClient,
      IGameProvider gameProvider,
      IVerification verification,
      IPlayer player,
      IPlayerGameBalance playerGameBalance)
    {
        _gameFundTransferClient = gameFundTransferClient ?? throw new ArgumentNullException(nameof(gameFundTransferClient));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _verification = verification ?? throw new ArgumentNullException(nameof(verification));
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

        await _verification
            .VerifyPlayerFundAsync(
                player: player,
                gameProvider: gameProvider,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderBalance = await _playerGameBalance
            .GetBalanceAsync(
            gameProviderCode: gameProvider.GameProviderCode,
            playerCode: player.PlayerCode,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var freeBalance = gameProviderBalance.FreeBalance;

        if (freeBalance - command.Amount < 0)
        {
            throw new Exception($"Insufficient game wallet balance.({freeBalance})");
        }

        var amount = command.Amount;

        if (gameProvider.GameProviderCode == GameProviderType.SM01)
            amount = Math.Floor(amount);

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
