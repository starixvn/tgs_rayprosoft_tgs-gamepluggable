using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Tgs.GameCommand.Client;
using Tgs.GameProviders.Pluggable.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProviderRegisterExtensions
    {
        public static async Task<WebApplication> UseGameProviderRegister(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();
                var gameProvider = scope.ServiceProvider.GetRequiredService<IGameProvider>();

                var gameProviderIds = await client
                                .GameProviderGetGameProviderIdsAsync()
                                .ToArrayAsync()
                                .ConfigureAwait(false);

                foreach (var gameProviderId in gameProviderIds)
                {
                    _ = await gameProvider
                            .GetAsync(
                                gameProviderId.GameProviderId)
                            .ConfigureAwait(false);
                    _ = await gameProvider
                            .GetGameProviderIdAsync(
                                gameProviderId.GameProviderCode)
                            .ConfigureAwait(false);
                }
            }

            return app;
        }
    }
}
