using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class FundInCommandService(
	IGameFundTransferClient client,
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IPlayerGameBalance playerGameBalance) : ICommandService<FundInCommand>
{
	public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
	{
		var playerInfo = await player
			.GetAsync(
				command.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProvider
			.GetGameProviderIdAsync(
				command.GameProviderCode,
				cancellationToken)
			.ConfigureAwait(false);

		var gameProviderInfo = await gameProvider
			.GetAsync(
				gameProviderId,
				cancellationToken)
			.ConfigureAwait(false);

		if (!gameProviderInfo.TransferWallet)
		{
			throw new Exception($"This game provider does not provide multi-wallet functionality.({gameProviderInfo.GameProviderCode})");
		}

		var gameProviderBalance = await playerGameBalance
			.GetBalanceAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			currencyCode: playerInfo.CurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var amount = gameProviderBalance.FreeBalance;

		if (gameProviderInfo.GameProviderCode != GameProviderType.SM01)
		{
			amount *= 100;
			amount = Math.Floor(amount);

			if (amount > 0)
			{
				amount /= 100;
			}
		}
		else
		{
			amount = Math.Floor(amount);
		}

		if (amount == 0)
			return;

		_ = await client
			.FundInResultAsync(
				gameProviderId: gameProviderInfo.GameProviderId,
				gameProviderCode: gameProviderInfo.GameProviderCode,
				brandCode: playerInfo.BrandCode,
				playerCode: playerInfo.PlayerCode,
				walletId: playerInfo.WalletId,
				currencyCode: playerInfo.CurrencyCode,
				amount: amount,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
