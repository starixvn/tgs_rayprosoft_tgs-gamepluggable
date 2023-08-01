using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class GameSM01PlatformLoginGetService : IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>
{
    private readonly IGameCommandClient _gameCommandClient;
    private readonly IGameProvider _gameProvider;
    private readonly IGameUrl _gameUrl;
    private readonly IPlayer _player;
    private readonly IVerification _verification;

    public GameSM01PlatformLoginGetService(
        IGameUrl gameUrl,
        IVerification verification,
        IGameProvider gameProvider,
        IGameCommandClient gameCommandClient,
        IPlayer player)
    {
        _gameUrl = gameUrl ?? throw new ArgumentNullException(nameof(gameUrl));
        _verification = verification ?? throw new ArgumentNullException(nameof(verification));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameCommandClient = gameCommandClient ?? throw new ArgumentNullException(nameof(gameCommandClient));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<GameSM01PlatformLoginInfo> GetAsync(GameSM01PlatformLoginQuery query, CancellationToken cancellationToken = default)
    {
        var player = await _player
            .GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.SM01,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var games = await _gameCommandClient
            .GameQueryAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameInfo>();

        var game = games.ElementAt(0);

        await _verification
            .VerifyPlayerGameUrlAsync(
            player: player,
            game: game,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var info = await _gameUrl
            .GetGamePlatformLoginBySM01Async(
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameSM01PlatformLoginInfo(info.ApiUrl, info.MerchantId, info.CustomerName, info.Timestamp, info.Sign);
    }
}
