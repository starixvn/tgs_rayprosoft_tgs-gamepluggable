using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Core;

public interface IGame
{
    ValueTask<GameInfo> GetAsync(
        int gameId,
        CancellationToken cancellationToken = default);
}
