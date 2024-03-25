using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class FundOutCommandService(
    ICashServiceClient cashServiceClient,
    IGameFundTransferClient gameFundTransferClient,
    IGameProvider gameProvider,
    IVerification verification,
    IPlayer player,
    IPlayerGameBalance playerGameBalance) : ICommandService<FundOutCommand>
{
    public async ValueTask ExecuteAsync(FundOutCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount == 0)
        {
            return;
        }

        var playerInfo = await player
            .GetAsync(
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderId = await gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: command.GameProviderCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderInfo = await gameProvider
            .GetAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
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

        _ = await playerGameBalance
            .GetBalanceAsync(
            gameProviderInfo.GameProviderCode,
            playerInfo.PlayerCode,
            playerInfo.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerBalance = await cashServiceClient
            .GetBalanceAsync(
            walletId: playerInfo.DefaultWalletId,
            cancellationToken: cancellationToken)
            .AsTask()
            .WaitAsync(
            timeout: TimeSpan.FromSeconds(3),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (playerBalance - command.Amount < 0)
        {
            throw new Exception($"Insufficient balance in main wallet.({playerBalance})");
        }

        var amount = command.Amount;

        if (gameProviderInfo.GameProviderCode == GameProviderType.SM01)
            amount = Math.Floor(command.Amount);

        _ = await gameFundTransferClient
            .FundOutAsync(
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
