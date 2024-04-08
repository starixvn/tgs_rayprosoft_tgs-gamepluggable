using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Exceptions;

namespace Tgs.GamePluggable.Repositories;

internal class VerificationRepository(IGameCommandClient client) : IVerification
{
	public async ValueTask VerifyBrandCurrencyAsync(
		Guid brandId,
		Guid gameProviderId,
		string currencyCode,
		CancellationToken cancellationToken = default)
	{
		var brandCurrency = await client
			.BrandGetCurrencyAsync(
				brandId: brandId,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderCurrency = await client
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
		var resellerStatus = await client
			.ResellerGetStatusAsync(
			resellerId: player.ResellerId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var licenseeStatus = await client
			.LicenseeGetStatusAsync(
			licenseeId: player.LicenseeId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var brandStatus = await client
			.BrandGetStatusAsync(
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerStatus = await client
			.PlayerGetStatusAsync(
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderStatus = await client
			.GameProviderGetStatusAsync(
			gameProviderId: gameProvider.GameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (resellerStatus != ResellerStatus.Active ||
			licenseeStatus != LicenseeStatus.Active ||
			brandStatus != BrandStatus.Active ||
			playerStatus != PlayerStatus.Enabled &&
			playerStatus != PlayerStatus.Disabled ||
			gameProviderStatus != GameProviderStatus.Active
			)
		{
			throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		var brandCurrency = await client
			.BrandGetCurrencyAsync(
			brandId: player.BrandId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderCurrency = await client
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
		var resellerStatus = await client
			.ResellerGetStatusAsync(
			resellerId: player.ResellerId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var licenseeStatus = await client
			.LicenseeGetStatusAsync(
			licenseeId: player.LicenseeId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var brandStatus = await client
			.BrandGetStatusAsync(
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerStatus = await client
			.PlayerGetStatusAsync(
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderStatus = await client
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

		var brandCurrency = await client
			.BrandGetCurrencyAsync(
			brandId: player.BrandId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderCurrency = await client
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
		var resellerStatus = await client
			.ResellerGetStatusAsync(
				resellerId: player.ResellerId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var licenseeStatus = await client
			.LicenseeGetStatusAsync(
				licenseeId: player.LicenseeId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var brandStatus = await client
			.BrandGetStatusAsync(
				brandId: player.BrandId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerStatus = await client
			.PlayerGetStatusAsync(
				playerCode: player.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderStatus = await client
			.GameProviderGetStatusAsync(
				gameProviderId: game.GameProviderId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameStatus = await client
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

		var brandCurrency = await client
			.BrandGetCurrencyAsync(
				brandId: player.BrandId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderCurrency = await client
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
