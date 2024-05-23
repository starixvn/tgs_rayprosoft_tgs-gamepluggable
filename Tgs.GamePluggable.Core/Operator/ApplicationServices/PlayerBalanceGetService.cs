using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class PlayerBalanceGetService(
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IVerification verification,
	IGameFundTransferClient gameFundTransferClient) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
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

		var fundBalanceReply = await gameFundTransferClient.FundBalanceAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceResult(
			Balance: fundBalanceReply.Balance,
			FreeBalance: fundBalanceReply.FreeBalance,
			OutstandingBalance: fundBalanceReply.OutstandingBalance);
	}
}
