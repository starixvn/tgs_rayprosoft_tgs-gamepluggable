using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersOperatorPluggableClient
    {
        ValueTask<GameProviderFundResultViewModel?> GameProviderFundInAsync(string playerCode, string gameProviderCode, decimal amount);

        ValueTask<GameProviderFundResultViewModel?> GameProviderFundOutAsync(string playerCode, string gameProviderCode, decimal amount);

        ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode);

        ValueTask<Uri?> GetGameUrlAsync(string playerCode, Guid loginRecordId, bool isMobile, int gameId, string language, string ipAddress, Uri backUrl);
    }
}
