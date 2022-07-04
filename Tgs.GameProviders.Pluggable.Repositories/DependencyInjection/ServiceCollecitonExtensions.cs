using System;
using Microsoft.EntityFrameworkCore;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories;
using Tgs.GameProviders.Pluggable.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddDbBetRepository(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
            => services
                .AddSerialNumberGenerator()
                .AddDbContext<BetContext>(efConfigure)
                .AddTransient<IRoundRepository, RoundRepository>();
    }
}
