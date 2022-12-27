using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class GameSM01PlatformLoginGetService : IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>
{
    private readonly IGameProvider _gameProvider;
    private readonly IGameUrl _gameUrl;
    private readonly IGameVerificationClient _gameVerificationClient;
    private readonly IGameProvidersQueryServerClient _queryServerClient;

    public GameSM01PlatformLoginGetService(
        IGameUrl gameUrl,
        IGameVerificationClient gameVerificationClient,
        IGameProvider gameProvider,
        IGameProvidersQueryServerClient queryServerClient)
    {
        _gameUrl = gameUrl ?? throw new ArgumentNullException(nameof(gameUrl));
        _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _queryServerClient = queryServerClient ?? throw new ArgumentNullException(nameof(queryServerClient));
    }

    public async ValueTask<GameSM01PlatformLoginInfo> GetAsync(GameSM01PlatformLoginQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
                       .GetGameProviderIdAsync(
                           GameProviderType.SM01,
                           cancellationToken)
                       .ConfigureAwait(false);

        var games = await _queryServerClient
            .GameQueryAsync(gameProviderId, cancellationToken)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var gameId = games[0].GameId;

        if (!await _gameVerificationClient.VerifyPlayerGameUrlAsync(query.PlayerCode, gameId).ConfigureAwait(false))
        {
            throw new Exception($"Failed to get game URL. (1. The player does not exist or is not enabled. 2. The game does not exist or is not enabled. 3. The game does not support this currency.)");
        }

        var info = await _gameUrl
                .GetGamePlatformLoginBySM01Async(query.PlayerCode, cancellationToken)
                .ConfigureAwait(false);

        return new GameSM01PlatformLoginInfo(info.MerchantId, info.CustomerName, info.Timestamp, info.Sign);
    }
}
