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
	IVerification verification,
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

		await verification.VerifyBackofficePlayerFundInAsync(
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
			gameProviderCode: gameProviderInfo.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var amount = fundTransferBalanceResult.FreeBalance;

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

		_ = await gameFundTransferClient.FundTransferInAsync(
			playerCode: playerInfo.PlayerCode,
			gameProviderCode: gameProviderInfo.GameProviderCode,
			amount: amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
