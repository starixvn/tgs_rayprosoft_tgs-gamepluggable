using System.Threading;
using System.Threading.Tasks;

using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class PlayerBalanceGetService(
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IVerification verification,
	IPlayerGameBalance playerGameBalance) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			code: query.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderInfo = await gameProviderRepository.GetAsync(
			id: gameProviderId,
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
			currencyCode: playerInfo.CurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceResult(
			Balance: result.Balance,
			FreeBalance: result.FreeBalance,
			OutstandingBalance: result.OutstandingBalance);
	}
}
