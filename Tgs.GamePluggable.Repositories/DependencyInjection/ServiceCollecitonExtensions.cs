using Microsoft.EntityFrameworkCore;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Entities;
using Tgs.GamePluggable.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGamePluggableRepositories(
		this IServiceCollection services,
		 Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<IPlayerRepository, PlayerRepository>()
		.Decorate<IPlayerRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerCacheRepository>(sp, innerRepository, TimeSpan.FromMinutes(30)))
		.AddTransient<IGameRepository, GameRepository>()
		.Decorate<IGameRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameCacheRepository>(sp, innerRepository, TimeSpan.FromMinutes(30)))
		.AddTransient<IGameProviderRepository, GameProviderRepository>()
		.Decorate<IGameProviderRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameProviderCacheRepository>(sp, innerRepository))
		.AddTransient<IPlayerGameBalance, PlayerGameBalance>()
		.AddTransient<IBrandRepository, BrandRepository>()
		.AddTransient<IGameUrl, GameUrlRepository>()
		.AddScoped<IVerification, VerificationRepository>()
		.Decorate<IVerification>((innerRepository, sp) => ActivatorUtilities.CreateInstance<VerificationCacheRepository>(sp, innerRepository, TimeSpan.FromMinutes(10)));
}
