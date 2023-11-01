using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GamePlayerLogOut.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class PlayerLogOutGameProviderCommandService : ICommandService<PlayerLogOutGameProviderCommand>
{
    private readonly IGamePlayerLogOutClient _client;

    public PlayerLogOutGameProviderCommandService(IGamePlayerLogOutClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(PlayerLogOutGameProviderCommand command, CancellationToken cancellationToken = default)
        => _client.LogOutAsync(
            playerCode: command.PlayerCode,
            gameProviderCode: command.GameProviderCode,
            cancellationToken: cancellationToken);
}
