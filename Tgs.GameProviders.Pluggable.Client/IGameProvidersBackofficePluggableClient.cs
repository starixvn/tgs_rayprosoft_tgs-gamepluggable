using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersBackofficePluggableClient
    {

        ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode);

        Task KickPlayerAsync(string playerCode);


        Task KickPlayerAsync(string playerCode, string gameProviderCode);

        IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId);

        ValueTask GameRoundClosedAsync(Guid RoundId, GameRoundClosedViewModel viewModel);

        ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId);

        ValueTask<JsonDocument?> GameRoundDetailGetAsync(Guid roundId, string language);
    }
}
