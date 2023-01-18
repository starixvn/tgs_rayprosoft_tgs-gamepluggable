using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameRepository : IGame
{
    private readonly IGameCommandClient _client;

    public GameRepository(IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameInfo> GetAsync(
        int gameId,
        CancellationToken cancellationToken = default)
        => await _client
        .GameGetAsync(
            gameId,
            cancellationToken)
        .ConfigureAwait(false)
        ?? throw new ArgumentException($"The game doesn't exist.{gameId}");
}
