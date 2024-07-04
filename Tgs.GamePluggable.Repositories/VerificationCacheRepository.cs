using Microsoft.Extensions.Caching.Memory;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class VerificationCacheRepository(
	IVerification verification,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IVerification
{
	public async ValueTask VerifyBackofficePlayerFundInAsync(Guid brandId, string brandCode, Guid gameProviderId, string gameProviderCode, bool transferWallet, string currencyCode, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "backoffice", ":", "player", ":", "fundin", ":", brandCode, ":", gameProviderCode, ":", currencyCode);
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification.VerifyBackofficePlayerFundInAsync(
				brandId: brandId,
				brandCode: brandCode,
				gameProviderId: gameProviderId,
				gameProviderCode: gameProviderCode,
				transferWallet: transferWallet,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyBackofficePlayerBalanceAsync(Guid brandId, string brandCode, Guid gameProviderId, string gameProviderCode, bool transferWallet, string currencyCode, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "backoffice", ":", "player", ":", "balance", ":", brandCode, ":", gameProviderCode, ":", currencyCode);
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification.VerifyBackofficePlayerBalanceAsync(
				brandId: brandId,
				brandCode: brandCode,
				gameProviderId: gameProviderId,
				gameProviderCode: gameProviderCode,
				transferWallet: transferWallet,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyPlayerBalanceAsync(Core.Models.PlayerInfo player, Core.Models.GameProviderInfo gameProvider, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "balance", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);
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
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "fund", ":", player.PlayerCode, ":", gameProvider.GameProviderCode);
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
		var key = string.Concat(typeof(VerificationCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "verify", ":", "player", ":", "gameurl", ":", player.PlayerCode, ":", game.GameProviderCode, ":", game.GameId);
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
