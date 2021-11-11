using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersServicesConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IPlayerVerifyService, PlayerVerifyService>();
        }
    }
}
