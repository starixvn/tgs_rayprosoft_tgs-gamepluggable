using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IGameUrl
{
	ValueTask<Uri> GetGameUrlAsync(string gameProviderCode, string playerCode, int gameId, Guid loginRecordId, string language, GameProviders.Domains.PlatformType platformType, string ipAddress, Uri? backUrl, CancellationToken cancellationToken = default);
}
