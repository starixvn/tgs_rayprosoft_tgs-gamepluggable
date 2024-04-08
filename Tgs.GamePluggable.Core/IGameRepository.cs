using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IGameRepository
{
	ValueTask<Models.GameInfo> GetAsync(int id, CancellationToken cancellationToken = default);

	IAsyncEnumerable<int> QueryAsync(Guid gameProviderId, CancellationToken cancellationToken = default);
}
