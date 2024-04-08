using System;
using Microsoft.EntityFrameworkCore;
using Tgs.CashService.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Client;
using Tgs.GameRounds.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGamePluggable(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
		Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions,
		Action<GameFundTransferClientOptions, IServiceProvider> gameTransferClientOptions,
		Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions,
		Action<GameRoundsClientOptions, IServiceProvider> gameRoundsConfigure)
		=> services
		.AddMemoryCache()
		.AddGamePluggableCore(
			cashServiceClientOptions: cashServiceClientOptions,
			gameProvidersClientOptions: gameProvidersClientOptions,
			gameTransferClientOptions: gameTransferClientOptions,
			gameCurrencyClientOptions: gameCurrencyClientOptions,
			gameRoundsConfigure: gameRoundsConfigure)
		.AddGamePluggableRepositories(
			dbContextOptions: dbContextOptions);
}
