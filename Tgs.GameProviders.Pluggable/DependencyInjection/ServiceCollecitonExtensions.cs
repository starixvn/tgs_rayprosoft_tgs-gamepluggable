using System;
using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePlayerLogOut.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameProvidersPluggable(
        this IServiceCollection services,
        Action<CashServiceClientOptions, IServiceProvider> cashServiceClientOptions,
        Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions,
        Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
        Action<GamePlayerLogOutClientOptions, IServiceProvider> gamePlayerLogOutClientOptions,
        Action<GameVerificationClientOptions, IServiceProvider> gameVerificationClientOptions,
        Action<GameFundTransferClientOptions, IServiceProvider> gameTransferClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions,
        Action<GameRoundsClientOptions, IServiceProvider> gameRoundsConfigure)
        => services.AddGameProvidersPluggableCore(
            cashServiceClientOptions,
            gameProvidersClientOptions,
            gameProvidersQueryServerClientOptions,
            gamePlayerLogOutClientOptions,
            gameVerificationClientOptions,
            gameTransferClientOptions,
            gameCurrencyClientOptions,
            gameRoundsConfigure)
        .AddGameProvidersPluggableRepositories();
}
