using System;
using System.Reflection;
using Tgs.CashService.Client;
using Tgs.GameCommand.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Client;
using Tgs.GameRounds.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameProvidersPluggableCore(
        this IServiceCollection services,
        Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
        Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameFundTransferClientOptions, IServiceProvider> gameTransferClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions,
        Action<GameRoundsClientOptions, IServiceProvider> gameRoundsConfigure)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddCashServiceClient(cashServiceClientOptions)
        .AddGameProvidersClient(gameProvidersClientOptions)
        .AddGameCommandClient(gameCommandClientOptions)
        .AddGameFundTransferClient(gameTransferClientOptions)
        .AddGameCurrencyClient(gameCurrencyClientOptions)
        .AddGameRoundsClient(gameRoundsConfigure)
        .AddGamePlayerLogOutClient()
        .AddGameRecordHandleClient();
}
