using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class GameProviderRepository(
	IGameCommandClient client) : IGameProvider
{
	public async ValueTask<GameProviderInfo> GetAsync(
		Guid gameProviderId,
		CancellationToken cancellationToken = default)
		=> await client
		.GameProviderGetAsync(
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentException($"game provider doesn't exist.{gameProviderId}");

	public async ValueTask<GameProviderConfigInfo> GetConfigurationAsync(
		Guid gameProviderConfigId,
		CancellationToken cancellationToken = default)
		=> await client
		.GameProviderConfigurationGetAsync(
			gameProviderConfigId: gameProviderConfigId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentException($"game provider config doesn't exist.{gameProviderConfigId}");

	public async ValueTask<Guid> GetGameProviderIdAsync(
		string gameProviderCode,
		CancellationToken cancellationToken = default)
		=> await client
		.GameProviderGetGameProviderIdAsync(
			gameProviderCode: gameProviderCode,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new Exception($"game provider does not exist.({gameProviderCode})");
}
