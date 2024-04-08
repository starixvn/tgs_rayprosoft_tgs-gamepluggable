using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class FundInCommandService(
  IGameFundTransferClient gameFundTransferClient,
  IGameProvider gameProvider,
  IVerification verification,
  IPlayer player,
  IPlayerGameBalance playerGameBalance) : ICommandService<FundInCommand>
{
	public async ValueTask ExecuteAsync(FundInCommand command, CancellationToken cancellationToken = default)
	{
		if (command.Amount == 0)
		{
			return;
		}

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

		await verification
			.VerifyPlayerFundAsync(
				player: playerInfo,
				gameProvider: gameProviderInfo,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderBalance = await playerGameBalance
			.GetBalanceAsync(
			gameProviderCode: gameProviderInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var freeBalance = gameProviderBalance.FreeBalance;

		if (freeBalance - command.Amount < 0)
		{
			throw new Exception($"Insufficient game wallet balance.({freeBalance})");
		}

		var amount = command.Amount;

		if (gameProviderInfo.GameProviderCode == GameProviderType.SM01)
			amount = Math.Floor(amount);

		_ = await gameFundTransferClient
			.FundInAsync(
				gameProviderId: gameProviderInfo.GameProviderId,
				gameProviderCode: gameProviderInfo.GameProviderCode,
				brandCode: playerInfo.BrandCode,
				playerCode: playerInfo.PlayerCode,
				walletId: playerInfo.DefaultWalletId,
				currencyCode: playerInfo.DefaultWalletCurrencyCode,
				amount: amount,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
