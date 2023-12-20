using Tgs.GameCommand.Client;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class BrandRepository(IGameCommandClient client) : IBrand
{
    public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
        => await client.BrandGetGameProviderConfigurationIdAsync(
            brandId: brandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new ArgumentException($"Then brand game provider configuration doesn't exist.({brandId}|{gameProviderId})");
}
