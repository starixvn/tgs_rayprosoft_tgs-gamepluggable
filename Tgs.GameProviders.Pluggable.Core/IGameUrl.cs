using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameUrl
    {
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
}
