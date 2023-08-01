using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Pluggable.Core;

public interface IVerification
{
    ValueTask VerifyBrandCurrencyAsync(
        Guid brandId,
        Guid gameProviderId,
        string currencyCode,
        CancellationToken cancellationToken = default);

    ValueTask VerifyPlayerAsync(
        PlayerInfo player,
        GameProviderInfo gameProvider,
        CancellationToken cancellationToken = default);

    ValueTask VerifyPlayerBalanceAsync(
            PlayerInfo player,
        GameProviderInfo gameProvider,
        CancellationToken cancellationToken = default);

    ValueTask VerifyPlayerGameUrlAsync(
        PlayerInfo player,
        GameInfo game,
        CancellationToken cancellationToken = default);
}
