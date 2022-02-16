using System;
using System.Reflection;
using Tgs.CashService.Client;
using Tgs.GameProviders.KickPlayerServer.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.QueryServer.Client;
using TGS.GameProviders.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameProvidersCore(
            this IServiceCollection services,
            Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
            Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions,
            Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
            Action<KickPlayerServiceClientOptions, IServiceProvider> kickPlayerServiceClientOptions
            )
            => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddMemoryCache()
            .AddCashServiceClient(cashServiceClientOptions)
            .AddGameProvidersClient(gameProvidersClientOptions)
            .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
            .AddKickPlayerServerClient(kickPlayerServiceClientOptions)
            .AddTransient<IBrandService, BrandService>()
            .AddTransient<IGameProviderService, GameProviderService>()
            .AddTransient<IGameService, GameService>()
            .AddTransient<ILicenseeService, LicenseeService>()
            .AddTransient<IPlayerService, PlayerService>()
            .AddTransient<IResellerService, ResellerService>()
            .AddScoped<IPlayerVerifyHanlde, PlayerVerifyHanlde>();
    }
}
