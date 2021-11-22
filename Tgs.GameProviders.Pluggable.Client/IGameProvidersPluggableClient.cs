using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersPluggableClient
    {
        IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId);

        ValueTask GameRoundClosedAsync(Guid RoundId, GameRoundClosedViewModel viewModel);

        ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId);

        ValueTask KickPlayerAsync(string playerCode);

        ValueTask KickPlayerAsync(string playerCode, string gameProviderCode);
    }
}
