using Microsoft.EntityFrameworkCore;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Exceptions;

namespace Tgs.GamePluggable.Repositories;

internal class VerificationRepository(
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository) : IVerification
{
	public async ValueTask VerifyBrandCurrencyAsync(Guid brandId, Guid gameProviderId, string currencyCode, CancellationToken cancellationToken = default)
	{
		if (!await brandRepository.GetCurrencyExistAsync(brandId, currencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyBrandCurrencyException(brandId, gameProviderId, currencyCode);
		}

		if (!await gameProviderRepository.GetCurrencyExistAsync(gameProviderId, currencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyBrandCurrencyException(brandId, gameProviderId, currencyCode);
		}
	}

	public async ValueTask VerifyPlayerBalanceAsync(Core.Models.PlayerInfo player, Core.Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default)
	{
		if (!gameProvider.TransferWallet)
		{
			throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await playerRepository.GetStatusAsync(playerCode: player.PlayerCode, cancellationToken: cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await brandRepository.GetGameProviderStatusAsync(player.BrandId, gameProvider.GameProviderId, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await brandRepository.GetCurrencyExistAsync(player.BrandId, player.CurrencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await gameProviderRepository.GetCurrencyExistAsync(gameProvider.GameProviderId, player.CurrencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerBalanceException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}
	}

	public async ValueTask VerifyPlayerFundAsync(Core.Models.PlayerInfo player, Core.Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default)
	{
		if (!gameProvider.TransferWallet)
		{
			throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await playerRepository.GetStatusAsync(playerCode: player.PlayerCode, cancellationToken: cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await brandRepository.GetGameProviderStatusAsync(player.BrandId, gameProvider.GameProviderId, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await brandRepository.GetCurrencyExistAsync(player.BrandId, player.CurrencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}

		if (!await gameProviderRepository.GetCurrencyExistAsync(gameProvider.GameProviderId, player.CurrencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerFundException(player.BrandCode, player.PlayerCode, player.Username, gameProvider.GameProviderCode);
		}
	}

	public async ValueTask VerifyPlayerGameUrlAsync(Core.Models.PlayerInfo player, Core.Models.GameInfo game, CancellationToken cancellationToken = default)
	{
		if (!await playerRepository.GetStatusAsync(playerCode: player.PlayerCode, cancellationToken: cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
		}

		if (!await brandRepository.GetGameStatusAsync(player.BrandId, game.GameId, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
		}

		if (!await brandRepository.GetCurrencyExistAsync(player.BrandId, player.CurrencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
		}

		if (!await gameProviderRepository.GetCurrencyExistAsync(game.GameProviderId, player.CurrencyCode, cancellationToken).ConfigureAwait(false))
		{
			throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
		}

		var availableCurrencies = game.AvailableCurrencies ?? Enumerable.Empty<string>();

		if (availableCurrencies.Any())
		{
			if (!availableCurrencies.Contains(player.CurrencyCode))
			{
				throw new VerifyPlayerGameUrlException(player.BrandCode, player.PlayerCode, player.Username, game.GameProviderCode, game.GameCode);
			}
		}
	}
}
