using System.Reflection;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameProvidersPluggableRepositories(
        this IServiceCollection services)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddTransient<IPlayer, PlayerRepository>()
        .Decorate<IPlayer>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerCacheRepository>(sp, innerRepository, TimeSpan.FromMinutes(30)))
        .AddTransient<IGame, GameRepository>()
        .Decorate<IGame>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameCacheRepository>(sp, innerRepository, TimeSpan.FromMinutes(30)))
        .AddTransient<IGameProvider, GameProviderRepository>()
        .Decorate<IGameProvider>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameProviderCacheRepository>(sp, innerRepository))
        .AddTransient<IPlayerGameBalance, PlayerGameBalance>()
        .AddTransient<IBrand, BrandRepository>()
        .AddTransient<IGameUrl, GameUrlRepository>();
}
