using Tgs.GameCurrency.Client;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Models;
using Tgs.GameProviders.Client;

namespace Tgs.GamePluggable.Repositories;

internal class PlayerGameBalance(
	IGameProvidersClient gameProvidersClient,
	IGameCurrencyClient gameCurrencyClient) : IPlayerGameBalance
{
	public async ValueTask<PlatformBalanceInfo> GetBalanceAsync(string gameProviderCode, string playerCode, string currencyCode, CancellationToken cancellationToken = default)
	{
		var response = await gameProvidersClient
			.PlayerBalanceAsync(
			gameProviderCode: gameProviderCode,
			playerCode: playerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (response.Balance == 0)
		{
			return new PlatformBalanceInfo(
				Balance: 0.0m,
				FreeBalance: 0.0m,
				OutstandingBalance: 0.0m);
		}

		var result = await gameCurrencyClient
			.ConvertPlatformBalanceAsync(
			gameProviderCode: gameProviderCode,
			currencyCode: currencyCode,
			balance: response.Balance,
			freeBalance: response.FreeBalance,
			outstandingBalance: response.OutstandingBalance,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlatformBalanceInfo(
			Balance: result.Balance,
			FreeBalance: result.FreeBalance,
			OutstandingBalance: result.OutstandingBalance);
	}
}
