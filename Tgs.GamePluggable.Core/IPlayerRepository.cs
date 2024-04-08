using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IPlayerRepository
{
	ValueTask<Models.PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<bool> GetStatusAsync(string playerCode, CancellationToken cancellationToken = default);
}
