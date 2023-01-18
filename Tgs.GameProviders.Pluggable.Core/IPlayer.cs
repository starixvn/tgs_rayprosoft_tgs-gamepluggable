using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Pluggable.Core;

public interface IPlayer
{
    ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
