using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Exceptions;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class VerificationRepository : IVerification
{
    private readonly IGameCommandClient _client;

    public VerificationRepository(IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask VerifyBrandCurrencyAsync(
        Guid brandId,
        Guid gameProviderId,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var brandCurrency = await _client
            .BrandGetCurrencyAsync(
                brandId: brandId,
                currencyCode: currencyCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCurrency = await _client
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: currencyCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(brandCurrency) || string.IsNullOrWhiteSpace(gameProviderCurrency))
        {
            throw new VerifyBrandCurrencyException(brandId, gameProviderId, currencyCode);
        }
    }

    public async ValueTask VerifyPlayerBalanceAsync(
        PlayerInfo player,
        GameProviderInfo gameProvider,
        CancellationToken cancellationToken = default)
    {
        var resellerStatus = await _client
            .ResellerGetStatusAsync(
            resellerId: player.ResellerId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var licenseeStatus = await _client
            .LicenseeGetStatusAsync(
            licenseeId: player.LicenseeId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var brandStatus = await _client
            .BrandGetStatusAsync(
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerStatus = await _client
            .PlayerGetStatusAsync(
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderStatus = await _client
            .GameProviderGetStatusAsync(
            gameProviderId: gameProvider.GameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (resellerStatus != ResellerStatus.Active ||
            licenseeStatus != LicenseeStatus.Active ||
            brandStatus != BrandStatus.Active ||
            (playerStatus != PlayerStatus.Enabled &&
            playerStatus != PlayerStatus.Disabled) ||
            gameProviderStatus != GameProviderStatus.Active
            )
        {
            throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
        }

        var brandCurrency = await _client
            .BrandGetCurrencyAsync(
            brandId: player.BrandId,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCurrency = await _client
            .GameProviderGetCurrencyCodeAsync(
            gameProviderId: gameProvider.GameProviderId,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(brandCurrency) || string.IsNullOrWhiteSpace(gameProviderCurrency))
        {
            throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
        }
    }

    public async ValueTask VerifyPlayerFundAsync(
            PlayerInfo player,
        GameProviderInfo gameProvider,
        CancellationToken cancellationToken = default)
    {
        var resellerStatus = await _client
            .ResellerGetStatusAsync(
            resellerId: player.ResellerId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var licenseeStatus = await _client
            .LicenseeGetStatusAsync(
            licenseeId: player.LicenseeId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var brandStatus = await _client
            .BrandGetStatusAsync(
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerStatus = await _client
            .PlayerGetStatusAsync(
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderStatus = await _client
            .GameProviderGetStatusAsync(
            gameProviderId: gameProvider.GameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (resellerStatus != ResellerStatus.Active ||
            licenseeStatus != LicenseeStatus.Active ||
            brandStatus != BrandStatus.Active ||
            playerStatus != PlayerStatus.Enabled ||
            gameProviderStatus != GameProviderStatus.Active
            )
        {
            throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
        }

        var brandCurrency = await _client
            .BrandGetCurrencyAsync(
            brandId: player.BrandId,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCurrency = await _client
            .GameProviderGetCurrencyCodeAsync(
            gameProviderId: gameProvider.GameProviderId,
            currencyCode: player.DefaultWalletCurrencyCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(brandCurrency) || string.IsNullOrWhiteSpace(gameProviderCurrency))
        {
            throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
        }
    }

    public async ValueTask VerifyPlayerGameUrlAsync(
        PlayerInfo player,
        GameInfo game,
        CancellationToken cancellationToken = default)
    {
        var resellerStatus = await _client
            .ResellerGetStatusAsync(
                resellerId: player.ResellerId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var licenseeStatus = await _client
            .LicenseeGetStatusAsync(
                licenseeId: player.LicenseeId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var brandStatus = await _client
            .BrandGetStatusAsync(
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerStatus = await _client
            .PlayerGetStatusAsync(
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderStatus = await _client
            .GameProviderGetStatusAsync(
                gameProviderId: game.GameProviderId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameStatus = await _client
            .GameGetStatusAsync(
                gameid: game.GameId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (resellerStatus != ResellerStatus.Active ||
            licenseeStatus != LicenseeStatus.Active ||
            brandStatus != BrandStatus.Active ||
            playerStatus != PlayerStatus.Enabled ||
            gameProviderStatus != GameProviderStatus.Active ||
            gameStatus != GameStatus.Active
            )
        {
            throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
        }

        var brandCurrency = await _client
            .BrandGetCurrencyAsync(
                brandId: player.BrandId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCurrency = await _client
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: game.GameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(brandCurrency) || string.IsNullOrWhiteSpace(gameProviderCurrency))
        {
            throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
        }

        var availableCurrencies = game.AvailableCurrencies ?? Enumerable.Empty<string>();

        if (availableCurrencies.Any())
        {
            if (!availableCurrencies.Contains(brandCurrency))
            {
                throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
            }
        }
    }
}
