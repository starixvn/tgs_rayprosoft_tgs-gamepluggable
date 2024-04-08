using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IVerification
{
	ValueTask VerifyBrandCurrencyAsync(Guid brandId, Guid gameProviderId, string currencyCode, CancellationToken cancellationToken = default);

	ValueTask VerifyPlayerBalanceAsync(Models.PlayerInfo player, Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default);

	ValueTask VerifyPlayerFundAsync(Models.PlayerInfo player, Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default);

	ValueTask VerifyPlayerGameUrlAsync(Models.PlayerInfo player, Models.GameInfo game, CancellationToken cancellationToken = default);
}
