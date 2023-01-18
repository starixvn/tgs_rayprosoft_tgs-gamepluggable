using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class PlayerRepository : IPlayer
{
    private readonly IGameCommandClient _client;

    public PlayerRepository(
        IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
        => await _client
        .PlayerGetAsync(
            playerCode: playerCode,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"This player does not exist.({playerCode})");
}
