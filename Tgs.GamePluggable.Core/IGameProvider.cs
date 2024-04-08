using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GamePluggable.Core;

public interface IGameProvider
{
	ValueTask<GameProviderInfo> GetAsync(
		Guid gameProviderId,
		CancellationToken cancellationToken = default);

	ValueTask<GameProviderConfigInfo> GetConfigurationAsync(
		Guid gameProviderConfigId,
		CancellationToken cancellationToken = default);

	ValueTask<Guid> GetGameProviderIdAsync(
		string gameProviderCode,
		CancellationToken cancellationToken = default);
}
