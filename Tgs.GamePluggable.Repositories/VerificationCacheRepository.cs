using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class VerificationCacheRepository(
	IVerification verification,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IVerification
{
	public async ValueTask VerifyBrandCurrencyAsync(
		Guid brandId,
		Guid gameProviderId,
		string currencyCode,
		CancellationToken cancellationToken = default)
	{
		var key = $"game:pluggable:verificationcache:brand:currency:{brandId}:{gameProviderId}:{currencyCode}";
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification
				   .VerifyBrandCurrencyAsync(
					   brandId: brandId,
					   gameProviderId: gameProviderId,
					   currencyCode: currencyCode,
					   cancellationToken: cancellationToken)
				   .ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyPlayerBalanceAsync(
		PlayerInfo player,
		GameProviderInfo gameProvider,
		CancellationToken cancellationToken = default)
	{
		var key = $"game:pluggable:verificationcache:player:balance:{player.PlayerCode}:{gameProvider.GameProviderCode}";
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification
				.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}

	public async ValueTask VerifyPlayerFundAsync(
			PlayerInfo player,
		GameProviderInfo gameProvider,
		CancellationToken cancellationToken = default)
	{
		var key = $"game:pluggable:verificationcache:player:fund:{player.PlayerCode}:{gameProvider.GameProviderCode}";
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

	public async ValueTask VerifyPlayerGameUrlAsync(
		PlayerInfo player,
		GameInfo game,
		CancellationToken cancellationToken = default)
	{
		var key = $"game:pluggable:verificationcache:player:game:url:{player.PlayerCode}:{game.GameProviderCode}:{game.GameId}";
		if (!memoryCache.TryGetValue(key, out _))
		{
			await verification
				   .VerifyPlayerGameUrlAsync(
					   player: player,
					   game: game,
					   cancellationToken: cancellationToken)
				   .ConfigureAwait(false);

			_ = memoryCache.Set(key, DateTime.UtcNow, absoluteExpiration);
		}
	}
}
