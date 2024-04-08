using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GamePluggable.Core;

public interface IGame
{
	ValueTask<GameInfo> GetAsync(
		int gameId,
		CancellationToken cancellationToken = default);
}
