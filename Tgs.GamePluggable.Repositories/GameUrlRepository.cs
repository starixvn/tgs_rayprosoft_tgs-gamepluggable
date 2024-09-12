using Tgs.GamePluggable.Core;
using Tgs.GameProviders.Client;

namespace Tgs.GamePluggable.Repositories;

internal class GameUrlRepository(
	IGameProvidersClient gameProvidersClient) : IGameUrl
{
	public async ValueTask<Uri> GetGameUrlAsync(string gameProviderCode,
		string playerCode,
		int gameId,
		Guid loginRecordId,
		string language,
		GameProviders.Domains.PlatformType platformType,
		string ipAddress, Uri? backUrl,
		object? extraInfo,
		CancellationToken cancellationToken = default)
		=> await gameProvidersClient.GameUrlAsync(
				gameProviderCode: gameProviderCode.Equals("SABASW02") ? "SABASW" : gameProviderCode,
				playerCode: playerCode,
				gameId: gameId,
				loginRecordId: loginRecordId.ToString(),
				language: language,
				platformType: platformType,
				ipAddress: ipAddress,
				backUrl: backUrl,
				extraInfo: extraInfo,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();
}
