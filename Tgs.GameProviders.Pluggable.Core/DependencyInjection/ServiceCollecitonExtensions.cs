using System;
using System.Reflection;
using Tgs.CashService.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.KickPlayerServer.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;
using Tgs.GameVerification.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameProvidersPluggableCore(
        this IServiceCollection services,
        Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
        Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions,
        Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
        Action<KickPlayerServiceClientOptions, IServiceProvider> kickPlayerServiceClientOptions,
        Action<GameVerificationClientOptions, IServiceProvider> gameVerificationClientOptions,
        Action<GameFundTransferClientOptions, IServiceProvider> gameTransferClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions,
        Action<GameRoundsClientOptions, IServiceProvider> gameRoundsConfigure)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddCashServiceClient(cashServiceClientOptions)
        .AddGameProvidersClient(gameProvidersClientOptions)
        .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
        .AddKickPlayerServerClient(kickPlayerServiceClientOptions)
        .AddGameVerificationClient(gameVerificationClientOptions)
        .AddGameFundTransferClient(gameTransferClientOptions)
        .AddGameCurrencyClient(gameCurrencyClientOptions)
        .AddGameRoundsClient(gameRoundsConfigure);
}
