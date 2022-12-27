using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GamePlayerLogOut.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class KickPlayerCommandService : ICommandService<KickPlayerCommand>
{
    private readonly IGamePlayerLogOutClient _client;

    public KickPlayerCommandService(IGamePlayerLogOutClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(KickPlayerCommand command, CancellationToken cancellationToken = default)
        => _ = await _client
        .LogOutAsync(
            command.PlayerCode,
            cancellationToken)
        .ConfigureAwait(false);
}
