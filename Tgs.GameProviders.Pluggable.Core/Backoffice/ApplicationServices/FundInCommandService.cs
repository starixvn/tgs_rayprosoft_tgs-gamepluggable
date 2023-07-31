using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class FundInCommandService : ICommandService<FundInCommand>
{
    private readonly IGameFundTransferClient _client;
    private readonly IGameProvider _gameProvider;
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;

    public FundInCommandService(
        IGameFundTransferClient client,
        IGameProvider gameProvider,
        IPlayer player,
        IPlayerGameBalance playerGameBalance)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
    }

    public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
    {
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

        var gameProviderBalance = await _playerGameBalance
            .GetBalanceAsync(
            gameProviderCode: gameProvider.GameProviderCode,
            playerCode: player.PlayerCode,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var amount = gameProviderBalance.FreeBalance;

        if (gameProvider.GameProviderCode != GameProviderType.SM01)
        {
            amount *= 100;
            amount = Math.Floor(amount);

            if (amount > 0)
            {
                amount /= 100;
            }
        }
        else
        {
            amount = Math.Floor(amount);
        }

        if (amount == 0)
            return;

        _ = await _client
            .FundInResultAsync(
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
