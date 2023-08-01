using System;
using System.Threading;
using System.Threading.Tasks;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IGameProvider _gameProvider;
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;
    private readonly IVerification _verification;

    public PlayerBalanceGetService(
        IPlayer player,
        IGameProvider gameProvider,
        IVerification verification,
        IPlayerGameBalance playerGameBalance)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _verification = verification ?? throw new ArgumentNullException(nameof(verification));
        _playerGameBalance = playerGameBalance ?? throw new ArgumentNullException(nameof(playerGameBalance));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var player = await _player
            .GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: query.GameProviderCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProvider = await _gameProvider
            .GetAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await _verification
            .VerifyPlayerBalanceAsync(
            player: player,
            gameProvider: gameProvider,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = await _playerGameBalance
            .GetBalanceAsync(
            gameProviderCode: gameProvider.GameProviderCode,
            playerCode: player.PlayerCode,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceResult(
            Balance: result.Balance,
            FreeBalance: result.FreeBalance,
            OutstandingBalance: result.OutstandingBalance);
    }
}
