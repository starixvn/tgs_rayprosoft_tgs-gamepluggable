using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class VerificationCacheRepository : IVerification
{
    private readonly TimeSpan _absoluteExpiration;
    private readonly IMemoryCache _memoryCache;
    private readonly IVerification _verification;

    public VerificationCacheRepository(
        IVerification verification,
        IMemoryCache memoryCache,
        TimeSpan absoluteExpiration
        )
    {
        _verification = verification ?? throw new ArgumentNullException(nameof(verification));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _absoluteExpiration = absoluteExpiration;
    }

    public async ValueTask VerifyBrandCurrencyAsync(
        Guid brandId,
        Guid gameProviderId,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:verificationcache:brand:currency:{brandId}:{gameProviderId}:{currencyCode}";
        if (!_memoryCache.TryGetValue(key, out _))
        {
            await _verification
                   .VerifyBrandCurrencyAsync(
                       brandId: brandId,
                       gameProviderId: gameProviderId,
                       currencyCode: currencyCode,
                       cancellationToken: cancellationToken)
                   .ConfigureAwait(false);

            _ = _memoryCache.Set(key, DateTime.UtcNow, _absoluteExpiration);
        }
    }

    public async ValueTask VerifyPlayerAsync(
        PlayerInfo player,
        GameProviderInfo gameProvider,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:verificationcache:player:{player.PlayerCode}:{gameProvider.GameProviderCode}";
        if (!_memoryCache.TryGetValue(key, out _))
        {
            await _verification
                .VerifyPlayerAsync(
                player: player,
                gameProvider: gameProvider,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _ = _memoryCache.Set(key, DateTime.UtcNow, _absoluteExpiration);
        }
    }

    public async ValueTask VerifyPlayerGameUrlAsync(
        PlayerInfo player,
        GameInfo game,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:verificationcache:player:game:url:{player.PlayerCode}:{game.GameProviderCode}:{game.GameId}";
        if (!_memoryCache.TryGetValue(key, out _))
        {
            await _verification
                   .VerifyPlayerGameUrlAsync(
                       player: player,
                       game: game,
                       cancellationToken: cancellationToken)
                   .ConfigureAwait(false);

            _ = _memoryCache.Set(key, DateTime.UtcNow, _absoluteExpiration);
        }
    }
}
