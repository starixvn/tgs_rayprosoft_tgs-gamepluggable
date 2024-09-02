using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

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

		await verification.VerifyBackofficePlayerBalanceAsync(
			brandId: playerInfo.BrandId,
			brandCode: playerInfo.BrandCode,
			gameProviderId: gameProviderInfo.GameProviderId,
			gameProviderCode: gameProviderInfo.GameProviderCode,
			transferWallet: gameProviderInfo.TransferWallet,
			currencyCode: playerInfo.CurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var fundTransferBalanceResult = await gameFundTransferClient.FundTransferBalanceAsync(
			playerCode: playerInfo.PlayerCode,
			gameProviderCode: query.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceResult(
			Balance: fundTransferBalanceResult.Balance,
			FreeBalance: fundTransferBalanceResult.FreeBalance,
			OutstandingBalance: fundTransferBalanceResult.OutstandingBalance);
	}
}
