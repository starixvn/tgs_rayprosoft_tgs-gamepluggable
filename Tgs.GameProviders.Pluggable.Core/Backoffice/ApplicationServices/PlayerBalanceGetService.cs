using System;
using System.Threading;
using System.Threading.Tasks;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IGameProvider _gameProvider;
    private readonly IPlayer _player;
    private readonly IPlayerGameBalance _playerGameBalance;
    private readonly IVerification _verification;

    public PlayerBalanceGetService(
        IGameProvider gameProvider,
        IPlayer player,
        IVerification verification,
        IPlayerGameBalance playerGameBalance)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
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

        await _verification
            .VerifyBrandCurrencyAsync(
            brandId: player.BrandId,
            gameProviderId: gameProviderId,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = await _playerGameBalance
            .GetBalanceAsync(
            gameProviderCode: query.GameProviderCode,
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
