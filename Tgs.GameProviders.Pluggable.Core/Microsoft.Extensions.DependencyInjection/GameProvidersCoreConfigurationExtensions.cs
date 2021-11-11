using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersCoreConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersCore(this IServiceCollection services)
            => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);
    }
}
