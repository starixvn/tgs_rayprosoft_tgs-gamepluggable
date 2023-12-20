using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GamePlayerLogOut.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class PlayerLogOutGameProviderCommandService(IGamePlayerLogOutClient client) : ICommandService<PlayerLogOutGameProviderCommand>
{
    public ValueTask ExecuteAsync(PlayerLogOutGameProviderCommand command, CancellationToken cancellationToken = default)
        => client.LogOutAsync(
            playerCode: command.PlayerCode,
            gameProviderCode: command.GameProviderCode,
            cancellationToken: cancellationToken);
}
