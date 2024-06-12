using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameFundTransfer.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class FundInCommandService(
	ILogger<FundInCommandService> logger,
	IPlayerRepository playerRepository,
	IGameProviderRepository gameProviderRepository,
	IVerification verification,
	IGameFundTransferClient gameFundTransferClient) : ICommandService<FundInCommand>
{
	public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
	{
		var amount = Math.Floor(command.Amount);

		if (amount is < 1 or > 10000000)
		{
			logger.LogWarning("The field Amount must be between 1 and 10000000.({amount})", command.Amount);
			return;
		}

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

		await verification.VerifyPlayerFundAsync(
			player: playerInfo,
			gameProvider: gameProviderInfo,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var fundBalanceReply = await gameFundTransferClient.FundBalanceAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		ArgumentOutOfRangeException.ThrowIfLessThan(fundBalanceReply.FreeBalance - amount, 0.0m);

		_ = await gameFundTransferClient.FundTransferInAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			walletId: playerInfo.WalletId,
			currencyCode: playerInfo.CurrencyCode,
			amount: amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
