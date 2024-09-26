using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public class GetRoundTicketDetailGetService(IGameProvidersClient providersClient) : IGetService<GetRoundTicketDetailQuery, JsonDocument?>
{
	public async ValueTask<JsonDocument?> GetAsync(GetRoundTicketDetailQuery query, CancellationToken cancellationToken = default)
	{
		return await providersClient.TicketDetailAsync(new()
		{
			SettleDate = query.SettleDate,
			ResultId = query.ResultId,
			BaseCurrency = query.BaseCurrency,
			ExternalRoundId = query.ExternalRoundId,
			GameProviderCode = query.GameProviderCode,
			PlayerCode = query.PlayerCode,
			GameId = query.GameId,
			Language = query.Language,
			PlayerUsername = query.PlayerUsername,
		}, cancellationToken).ConfigureAwait(false);
	}
}
