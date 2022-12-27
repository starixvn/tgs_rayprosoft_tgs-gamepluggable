using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GamePlayerLogOut.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class KickPlayerGameProviderCommandService : ICommandService<KickPlayerGameProviderCommand>
{
    private readonly IGamePlayerLogOutClient _client;

    public KickPlayerGameProviderCommandService(IGamePlayerLogOutClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(KickPlayerGameProviderCommand command, CancellationToken cancellationToken = default)
        => _ = await _client
        .LogOutByGameProviderAsync(
            command.PlayerCode,
            command.GameProviderCode,
            cancellationToken)
        .ConfigureAwait(false);
}
