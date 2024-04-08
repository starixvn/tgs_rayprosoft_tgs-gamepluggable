using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GamePluggable.Core;

public interface IPlayer
{
	ValueTask<PlayerInfo> GetAsync(
		string playerCode,
		CancellationToken cancellationToken = default);
}
