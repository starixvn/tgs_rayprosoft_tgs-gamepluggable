using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersOperatorPluggableClient
    {
        ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode);
    }
}
