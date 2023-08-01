using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

internal class GameUrlGetService : IGetService<GameUrlQuery, Uri>
{
    private readonly IGame _game;
    private readonly IGameUrl _gameUrl;
    private readonly IPlayer _player;
    private readonly IVerification _verification;

    public GameUrlGetService(
        IGame game,
        IGameUrl gameUrl,
        IVerification verification,
        IPlayer player)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameUrl = gameUrl ?? throw new ArgumentNullException(nameof(gameUrl));
        _verification = verification ?? throw new ArgumentNullException(nameof(verification));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<Uri> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var player = await _player
            .GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var game = await _game
            .GetAsync(
            gameId: query.GameId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await _verification
            .VerifyPlayerGameUrlAsync(
            player: player,
            game: game,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return game.GameProviderCode switch
        {
            var o when o == GameProviderType.AsiaGaming
            => await _gameUrl
                .GetGameUrlByAsiaGamingAsync(
                    game.GameProviderCode,
                    (GameProvidersClientGameType)game.Type,
                    game.GameId,
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
                    game.GameProviderCode,
                    (GameProvidersClientGameType)game.Type,
                    game.GameId,
                    player.BrandCode,
                    player.PlayerCode,
                    player.DefaultWalletCurrencyCode,
                    query.LoginRecordId,
                    query.PlatformType,
                    query.Language,
                    query.IpAddress,
                    query.BackUrl,
                    player.BrandId,
                    game.GameProviderId,
                    cancellationToken)
                .ConfigureAwait(false),
            _ => await _gameUrl
                .GetGameUrlAsync(
                    game.GameProviderCode,
                    (GameProvidersClientGameType)game.Type,
                    game.GameId,
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
