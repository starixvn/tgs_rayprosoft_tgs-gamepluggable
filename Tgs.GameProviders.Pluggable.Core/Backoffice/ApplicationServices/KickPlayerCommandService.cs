using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.KickPlayerServer.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class KickPlayerCommandService : ICommandService<KickPlayerCommand>
{
    private readonly IKickPlayerServiceClient _kickPlayerServiceClient;

    public KickPlayerCommandService(IKickPlayerServiceClient kickPlayerServiceClient)
    {
        _kickPlayerServiceClient = kickPlayerServiceClient ?? throw new ArgumentNullException(nameof(kickPlayerServiceClient));
    }

    public ValueTask ExecuteAsync(KickPlayerCommand command, CancellationToken cancellationToken = default)
        => _kickPlayerServiceClient.KickPlayerAsync(command.PlayerCode);
}
