using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class GameSM01PlatformLoginGetService : IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>
{
    private readonly IGameUrl _gameUrl;
    private readonly IGameVerificationClient _gameVerificationClient;

    public GameSM01PlatformLoginGetService(
        IGameUrl gameUrl,
        IGameVerificationClient gameVerificationClient)
    {
        _gameUrl = gameUrl ?? throw new ArgumentNullException(nameof(gameUrl));
        _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
    }

    public async ValueTask<GameSM01PlatformLoginInfo> GetAsync(GameSM01PlatformLoginQuery query, CancellationToken cancellationToken = default)
    {
        if (!await _gameVerificationClient.VerifyPlayerGameUrlAsync(query.PlayerCode, query.GameId).ConfigureAwait(false))
        {
            throw new Exception($"Failed to get game URL. (1. The player does not exist or is not enabled. 2. The game does not exist or is not enabled. 3. The game does not support this currency.)");
        }

        var info = await _gameUrl
                .GetGamePlatformLoginBySM01Async(query.PlayerCode, cancellationToken)
                .ConfigureAwait(false);

        return new GameSM01PlatformLoginInfo(info.MerchantId, info.CustomerName, info.Timestamp, info.Sign);
    }
}
