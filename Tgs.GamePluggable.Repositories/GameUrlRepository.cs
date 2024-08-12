using Tgs.GamePluggable.Core;
using Tgs.GameProviders.Client;

namespace Tgs.GamePluggable.Repositories;

internal class GameUrlRepository(
	IGameProvidersClient gameProvidersClient) : IGameUrl
{
	public async ValueTask<Uri> GetGameUrlAsync(string gameProviderCode, string playerCode, int gameId, Guid loginRecordId, string language, GameProviders.Domains.PlatformType platformType, string ipAddress, Uri? backUrl, CancellationToken cancellationToken = default)
		=> await gameProvidersClient.GameUrlAsync(
			gameProviderCode: gameProviderCode,
			playerCode: playerCode,
			gameId: gameId,
			loginRecordId: loginRecordId.ToString(),
			language: language,
			platformType: platformType,
			ipAddress: ipAddress,
			backUrl: backUrl,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new NullReferenceException();
}
