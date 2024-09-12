using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories;

internal class BrandGameProviderRepository(
	TgsContext context) : IBrandGameProviderRepository
{
	public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default)
	{
		var brandGameProvider = await context.BrandGameProviders
			.FirstOrDefaultAsync(
			predicate: bgp => bgp.BrandId == brandId && bgp.GameProviderId == gameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(new JsonObject
			{
				["BrandId"] = brandId,
				["GameProviderId"] = gameProviderId
			}.ToJsonString());

		return brandGameProvider.GameProviderConfigurationId;
	}
}
