using System.Threading;
using System.Threading.Tasks;

using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class PlayerBalanceGetService(
	IGameProvider gameProvider,
	IPlayer player,
	IVerification verification,
	IPlayerGameBalance playerGameBalance) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await player
			.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: query.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await verification
			.VerifyBrandCurrencyAsync(
			brandId: playerInfo.BrandId,
			gameProviderId: gameProviderId,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var result = await playerGameBalance
			.GetBalanceAsync(
			gameProviderCode: query.GameProviderCode,
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
