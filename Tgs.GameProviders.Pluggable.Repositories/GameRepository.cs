using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameRepository(IGameCommandClient client) : IGame
{
    public async ValueTask<GameInfo> GetAsync(
        int gameId,
        CancellationToken cancellationToken = default)
        => await client.GameGetAsync(
            gameId,
            cancellationToken)
        .ConfigureAwait(false)
        ?? throw new ArgumentException($"The game doesn't exist.{gameId}");
}
