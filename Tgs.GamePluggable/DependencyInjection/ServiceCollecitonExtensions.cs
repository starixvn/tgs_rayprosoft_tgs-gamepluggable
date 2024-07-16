using System;
using Microsoft.EntityFrameworkCore;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGamePluggable(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
		Action<GameFundTransferClientOptions, IServiceProvider> gameTransferClientOptions)
		=> services
		.AddMemoryCache()
		.AddGamePluggableCore(
			cashServiceClientOptions: cashServiceClientOptions,
			gameTransferClientOptions: gameTransferClientOptions)
		.AddGamePluggableRepositories(
			dbContextOptions: dbContextOptions);
}
