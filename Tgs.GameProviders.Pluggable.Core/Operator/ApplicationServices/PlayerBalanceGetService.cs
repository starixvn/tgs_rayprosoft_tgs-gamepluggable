using System;
using System.Threading;
using System.Threading.Tasks;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, decimal>
{
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;

    public PlayerBalanceGetService(
        IPlayer playerService,
        IPlayerGameBalance playerGameBalance)
    {
        _player = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
    }

    public async ValueTask<decimal> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var player = await _player
            .GetAsync(
                playerCode: query.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = await _playerGameBalance
            .GetBalanceAsync(
            gameProviderCode: query.GameProviderCode,
            playerCode: player.PlayerCode,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result.FreeBalance;
    }
}
