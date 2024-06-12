using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class FundOutCommandService(
	ILogger<FundInCommandService> logger,
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IVerification verification,
	ICashServiceClient cashServiceClient,
	IGameFundTransferClient gameFundTransferClient) : ICommandService<FundOutCommand>
{
	public async ValueTask ExecuteAsync(FundOutCommand command, CancellationToken cancellationToken = default)
	{
		var amount = Math.Floor(command.Amount);

		if (amount is < 1 or > 10000000)
		{
			logger.LogWarning("The field Amount must be between 1 and 10000000.({amount})", command.Amount);
			return;
		}

		var playerInfo = await playerRepository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			code: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderInfo = await gameProviderRepository.GetAsync(
			id: gameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!gameProviderInfo.TransferWallet)
		{
			throw new Exception($"This game provider does not provide multi-wallet functionality.({gameProviderInfo.GameProviderCode})");
		}

		await verification.VerifyPlayerFundAsync(
			player: playerInfo,
			gameProvider: gameProviderInfo,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerBalance = await cashServiceClient.GetBalanceAsync(
			walletId: playerInfo.WalletId,
			cancellationToken: cancellationToken)
			.AsTask()
			.WaitAsync(
			timeout: TimeSpan.FromSeconds(3),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		ArgumentOutOfRangeException.ThrowIfLessThan(playerBalance - amount, 0.0m);

		_ = await gameFundTransferClient.FundTransferOutAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			walletId: playerInfo.WalletId,
			currencyCode: playerInfo.CurrencyCode,
			amount: amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
