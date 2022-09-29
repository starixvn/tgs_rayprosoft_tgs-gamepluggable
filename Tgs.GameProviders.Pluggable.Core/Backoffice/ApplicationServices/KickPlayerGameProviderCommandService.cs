using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class KickPlayerGameProviderCommandService : ICommandService<KickPlayerGameProviderCommand>
{
    private readonly IGameProvidersClient _gameProvidersClient;

    public KickPlayerGameProviderCommandService(IGameProvidersClient gameProvidersClient)
    {
        _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
    }

    public ValueTask ExecuteAsync(KickPlayerGameProviderCommand command, CancellationToken cancellationToken = default)
        => _gameProvidersClient.KickPlayerAsync(command.GameProviderCode, command.PlayerCode, cancellationToken: cancellationToken);
}
