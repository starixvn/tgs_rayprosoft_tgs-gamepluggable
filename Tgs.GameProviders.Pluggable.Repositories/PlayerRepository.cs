using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class PlayerRepository : IPlayer
{
    private readonly IGameProvidersQueryServerClient _client;

    public PlayerRepository(
        IGameProvidersQueryServerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var data = await _client
            .PlayerGetAsync(
                playerCode,
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This player does not exist.({playerCode})");

        return new PlayerInfo(
               BrandId: data.BrandId,
               BrandCode: data.BrandCode,
               PlayerCode: data.PlayerCode,
               DefaultWalletId: data.DefaultWalletId,
               DefaultWalletCurrencyCode: data.DefaultWalletCurrencyCode);
    }
}
