using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersPluggableClient
    {
        IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId);

        ValueTask GameRoundClosedAsync(Guid RoundId, GameRoundClosedViewModel viewModel);

        ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId);

        ValueTask<JsonDocument?> GameRoundDetailGetAsync(Guid roundId, string language);

        ValueTask KickPlayerAsync(string playerCode);

        ValueTask KickPlayerAsync(string playerCode, string gameProviderCode);
    }
}
