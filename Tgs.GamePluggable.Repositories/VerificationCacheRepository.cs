using System;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories;

internal class VerificationCacheRepository(
	IVerification verification,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IVerification
{
	public async ValueTask VerifyBrandCurrencyAsync(Guid brandId, Guid gameProviderId, string currencyCode, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "brand", ":", "currency", ":", brandId, ":", gameProviderId, ":", currencyCode);
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification.VerifyBrandCurrencyAsync(
				brandId: brandId,
				gameProviderId: gameProviderId,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyPlayerBalanceAsync(Core.Models.PlayerInfo player, Core.Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "player", ":", "balance", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyPlayerFundAsync(Core.Models.PlayerInfo player, Core.Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "player", ":", "fund", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification
				.VerifyPlayerFundAsync(
				player: player,
				gameProvider: gameProvider,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyPlayerGameUrlAsync(Core.Models.PlayerInfo player, Core.Models.GameInfo game, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "player", ":", "gameurl", ":", player.PlayerCode, ":", game.GameProviderCode, ":", game.GameId);
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification.VerifyPlayerGameUrlAsync(
				player: player,
				game: game,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}
}
