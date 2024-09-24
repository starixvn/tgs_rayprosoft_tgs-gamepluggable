using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public class GetRoundTicketDetailGetService(IGameProvidersClient providersClient) : IGetService<GetRoundTicketDetailQuery, Uri?>
{
	public async ValueTask<Uri?> GetAsync(GetRoundTicketDetailQuery query, CancellationToken cancellationToken = default)
	{
		var result = await providersClient.TicketDetailUrlAsync(query.GameProviderCode, query.PlayerCode, query.ExternalId, query.Language, query.BaseCurrency, query.PlayerUsername, cancellationToken).ConfigureAwait(false);

		return result?.Url;
	}
}
