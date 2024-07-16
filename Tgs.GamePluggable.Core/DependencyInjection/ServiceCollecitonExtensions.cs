using System;
using System.Reflection;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGamePluggableCore(
		this IServiceCollection services,
		Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
		Action<GameFundTransferClientOptions, IServiceProvider> gameTransferClientOptions)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddCashServiceClient(cashServiceClientOptions)
		.AddGameFundTransferClient(gameTransferClientOptions);
}
