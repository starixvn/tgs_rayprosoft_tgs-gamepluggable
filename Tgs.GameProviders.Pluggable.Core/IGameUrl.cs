using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core;

public interface IGameUrl
{
    ValueTask<GameSM01PlatformLoginInfo> GetGamePlatformLoginBySM01Async(
        string playerCode,
        CancellationToken cancellationToken = default);

    ValueTask<Uri> GetGameUrlAsync(
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
        CancellationToken cancellationToken = default);

    ValueTask<Uri> GetGameUrlByAsiaGamingAsync(
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
        CancellationToken cancellationToken = default);

    ValueTask<Uri> GetGameUrlByBetConstructAsync(
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
        CancellationToken cancellationToken = default);
}
