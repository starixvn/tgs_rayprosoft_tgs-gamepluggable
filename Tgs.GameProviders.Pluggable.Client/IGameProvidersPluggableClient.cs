using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    public interface IGameProvidersPluggableClient
    {
        ValueTask ClosedGameRoundAsync(Guid RoundId, ClosedGameRoundViewModel viewModel);
    }
}
