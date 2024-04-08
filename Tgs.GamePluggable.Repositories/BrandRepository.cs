using Tgs.GameCommand.Client;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

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
