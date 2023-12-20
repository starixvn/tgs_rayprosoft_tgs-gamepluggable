using System;
using System.Threading;
using System.Threading.Tasks;

using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class PlayerBalanceGetService(
    IPlayer player,
    IGameProvider gameProvider,
    IVerification verification,
    IPlayerGameBalance playerGameBalance) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
    public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var playerInfo = await player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: query.GameProviderCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderInfo = await gameProvider.GetAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await verification.VerifyPlayerBalanceAsync(
            player: playerInfo,
            gameProvider: gameProviderInfo,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = await playerGameBalance.GetBalanceAsync(
            gameProviderCode: gameProviderInfo.GameProviderCode,
            playerCode: playerInfo.PlayerCode,
            currencyCode: playerInfo.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceResult(
            Balance: result.Balance,
            FreeBalance: result.FreeBalance,
            OutstandingBalance: result.OutstandingBalance);
    }
}
