using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameActionQueryService() : IQueryService<GameActionQuery, GameActionResult>
{
	public IAsyncEnumerable<GameActionResult> QueryAsync(GameActionQuery query, CancellationToken cancellationToken = default)
		=> Enumerable.Empty<GameActionResult>().ToAsyncEnumerable();
}
