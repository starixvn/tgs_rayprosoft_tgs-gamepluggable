using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersOperatorPluggableClient
    {
        ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode);
    }
}
