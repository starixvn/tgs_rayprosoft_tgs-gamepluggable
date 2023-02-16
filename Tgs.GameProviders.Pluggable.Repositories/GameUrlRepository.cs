using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameUrlRepository : IGameUrl
{
    private readonly IBrand _brand;
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IGameProvider _gameProvider;
    private readonly IGameProvidersClient _gameProvidersClient;

    public GameUrlRepository(
        IBrand brand,
        ICashServiceClient cashServiceClient,
        IGameProvider gameProvider,
        IGameProvidersClient gameProvidersClient)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
    }

    public async ValueTask<GameSM01PlatformLoginInfo> GetGamePlatformLoginBySM01Async(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var result = await _gameProvidersClient
            .GamePlatformLoginBySM01Async(
                playerCode,
                cancellationToken)
            .ConfigureAwait(false);

        return new GameSM01PlatformLoginInfo(result!.ApiUrl, result.MerchantId, result.CustomerName, result.Timestamp, result.Sign);
    }

    public async ValueTask<Uri> GetGameUrlAsync(
        string gameProviderCode,
        GameProvidersClientGameType gameType,
        int gameId,
        string brandCode,
        string playerCode,
        string currencyCode,
        Guid loginRecordId,
        GameProvidersClientPlatformType platformType,
        string language,
        string ipAddress,
        Uri? backUrl,
        CancellationToken cancellationToken = default)
        => await _gameProvidersClient
            .GameUrlAsync(
                gameProviderCode,
                gameType,
                gameId,
                brandCode,
                playerCode,
                currencyCode,
                loginRecordId,
                platformType,
                language,
                ipAddress,
                backUrl,
                cancellationToken)
        .ConfigureAwait(false)
        ?? throw new NullReferenceException();

    public async ValueTask<Uri> GetGameUrlByAsiaGamingAsync(
        string gameProviderCode,
        GameProvidersClientGameType gameType,
        int gameId,
        string brandCode,
        string playerCode,
        string currencyCode,
        Guid loginRecordId,
        GameProvidersClientPlatformType platformType,
        string language,
        string ipAddress,
        Uri? backUrl,
        Guid defaultWalletId,
        CancellationToken cancellationToken = default)
    {
        var playerBalance = await _cashServiceClient
                .GetBalanceAsync(
                    defaultWalletId,
                    cancellationToken)
                .ConfigureAwait(false);

        return await _gameProvidersClient
                .GameUrlByAsiaGamingAsync(
                    gameType,
                    gameId,
                    brandCode,
                    playerCode,
                    currencyCode,
                    loginRecordId,
                    platformType,
                    language,
                    ipAddress,
                    backUrl,
                    playerBalance,
                    cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException();
    }

    public async ValueTask<Uri> GetGameUrlByBetConstructAsync(
        string gameProviderCode,
        GameProvidersClientGameType gameType,
        int gameId,
        string brandCode,
        string playerCode,
        string currencyCode,
        Guid loginRecordId,
        GameProvidersClientPlatformType platformType,
        string language,
        string ipAddress,
        Uri? backUrl,
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var gameProviderConfigurationId = await _brand
            .GetGameProviderConfigurationIdAsync(
                brandId,
                gameProviderId,
                cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfiguration = await _gameProvider
            .GetConfigurationAsync(
                gameProviderConfigurationId,
                cancellationToken)
            .ConfigureAwait(false);

        var lobbyDomain = platformType switch
        {
            GameProvidersClientPlatformType.Desktop => gameProviderConfiguration.DesktopEndpoint,
            GameProvidersClientPlatformType.Mobile => gameProviderConfiguration.MobileEndpoint,
            _ => gameProviderConfiguration.DesktopEndpoint
        };

        return await _gameProvidersClient
            .GameUrlByBetConstructAsync(
                gameType,
                gameId,
                brandCode,
                playerCode,
                currencyCode,
                loginRecordId,
                platformType,
                language,
                ipAddress,
                backUrl,
                lobbyDomain,
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NullReferenceException();
    }
}
