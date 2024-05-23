using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class FundInCommandService(
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IGameFundTransferClient gameFundTransferClient) : ICommandService<FundInCommand>
{
	public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			command.GameProviderCode,
			cancellationToken)
			.ConfigureAwait(false);

		var gameProviderInfo = await gameProviderRepository.GetAsync(
			gameProviderId,
			cancellationToken)
			.ConfigureAwait(false);

		if (!gameProviderInfo.TransferWallet)
		{
			throw new Exception($"This game provider does not provide multi-wallet functionality.({gameProviderInfo.GameProviderCode})");
		}

		var fundBalanceReply = await gameFundTransferClient.FundBalanceAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var amount = fundBalanceReply.FreeBalance;

		if (amount > 10000000.00m)
		{
			amount = 10000000.00m;
		}

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

		_ = await gameFundTransferClient.FundInResultAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			walletId: playerInfo.WalletId,
			currencyCode: playerInfo.CurrencyCode,
			amount: amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
