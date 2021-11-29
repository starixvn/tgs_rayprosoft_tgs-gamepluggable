using System;
using Microsoft.EntityFrameworkCore;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories;
using Tgs.GameProviders.Pluggable.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryConfigurationExtensions
    {
        public static IServiceCollection AddDbBetRepository(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        {
            services.AddDbContext<BetContext>(efConfigure);

            return services
                .AddTransient<IRoundRepository, RoundRepository>();
        }

        public static IServiceCollection AddDbTgsRepository(this IServiceCollection services,
                    Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        {
            services.AddDbContext<TgsContext>(efConfigure);

            return services
                .AddTransient<IGameProviderRepository, GameProviderRepository>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<IResellerRepository, ResellerRepository>()
                .AddTransient<ILicenseeRepository, LicenseeRepository>()
                .AddTransient<IBrandRepository, BrandRepository>()
                .AddTransient<IPlayerRepository, PlayerRepository>()
                .AddTransient<IGameProviderFundTransferTransactionRepository, GameProviderFundTransferTransactionRepository>();
        }
    }
}
