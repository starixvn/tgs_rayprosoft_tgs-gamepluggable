using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameVerification.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class GameUrlGetService : IGetService<GameUrlQuery, Uri>
{
    private readonly IGame _game;
    private readonly IGameProvider _gameProvider;
    private readonly IGameUrl _gameUrl;
    private readonly IGameVerificationClient _gameVerificationClient;
    private readonly IPlayer _player;

    public GameUrlGetService(
        IGame game,
        IGameProvider gameProvider,
        IGameUrl gameUrl,
        IGameVerificationClient gameVerificationClient,
        IPlayer player)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameUrl = gameUrl ?? throw new ArgumentNullException(nameof(gameUrl));
        _gameVerificationClient = gameVerificationClient ?? throw new ArgumentNullException(nameof(gameVerificationClient));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<Uri> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var player = await _player
                        .GetAsync(
                            query.PlayerCode,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

        if (!await _gameVerificationClient.VerifyPlayerGameUrlAsync(query.PlayerCode, query.GameId).ConfigureAwait(false))
        {
            throw new Exception($"Failed to get game URL. (1. The player does not exist or is not enabled. 2. The game does not exist or is not enabled. 3. The game does not support this currency.)");
        }

        var game = await _game
                    .GetAsync(
                        query.GameId,
                        cancellationToken)
                    .ConfigureAwait(false);

        var gameProvider = await _gameProvider
                    .GetAsync(
                        game.GameProviderId,
                        cancellationToken)
                    .ConfigureAwait(false);

        return gameProvider.GameProviderCode switch
        {
            var o when o == GameProviderType.AsiaGaming
            => await _gameUrl
                .GetGameUrlByAsiaGamingAsync(
                    gameProvider.GameProviderCode,
                    (GameProvidersClientGameType)game.Type,
                    query.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl,
                    player.DefaultWalletId,
                    cancellationToken)
                .ConfigureAwait(false),
            var o when o == GameProviderType.BetConstruct
            => await _gameUrl
                .GetGameUrlByBetConstructAsync(
                    gameProvider.GameProviderCode,
                    (GameProvidersClientGameType)game.Type,
                    query.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl,
                    player.BrandId,
                    gameProvider.GameProviderId,
                    cancellationToken)
                .ConfigureAwait(false),
            _ => await _gameUrl
                .GetGameUrlAsync(
                    gameProvider.GameProviderCode,
                    (GameProvidersClientGameType)game.Type,
                    query.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl,
                    cancellationToken)
                .ConfigureAwait(false)
        };
    }
}
