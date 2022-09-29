using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class BrandRepository : IBrand
{
    private readonly IGameProvidersQueryServerClient _client;

    public BrandRepository(IGameProvidersQueryServerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var data = await _client
                    .BrandGetGameProviderConfigurationAsync(
                        brandId,
                        gameProviderId,
                        cancellationToken)
                    .ConfigureAwait(false)
                    ?? throw new ArgumentException($"Then brand game provider configuration doesn't exist.({brandId}|{gameProviderId})");

        return data.GameProviderConfigurationId;
    }
}
