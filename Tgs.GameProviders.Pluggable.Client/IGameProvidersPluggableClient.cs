using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersPluggableClient
    {
        ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId);

        IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId);

        ValueTask GameRoundClosedAsync(Guid RoundId, GameRoundClosedViewModel viewModel);
    }
}
