using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IVerification
{
	ValueTask VerifyBackofficePlayerFundInAsync(Guid brandId, string brandCode, Guid gameProviderId, string gameProviderCode, bool transferWallet, string currencyCode, CancellationToken cancellationToken = default);

	ValueTask VerifyBackofficePlayerBalanceAsync(Guid brandId, string brandCode, Guid gameProviderId, string gameProviderCode, bool transferWallet, string currencyCode, CancellationToken cancellationToken = default);

	ValueTask VerifyPlayerBalanceAsync(Models.PlayerInfo player, Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default);

	ValueTask VerifyPlayerFundAsync(Models.PlayerInfo player, Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default);

	ValueTask VerifyPlayerGameUrlAsync(Models.PlayerInfo player, Models.GameInfo game, CancellationToken cancellationToken = default);
}
