using System;
using Microsoft.Extensions.Options;
using Tgs.GameProviders.Pluggable.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameProvidersPluggableServerClient(
            this IServiceCollection services,
            Action<GameProvidersPluggableClientOptions, IServiceProvider> configure)
        {
            services
                .AddOptions<GameProvidersPluggableClientOptions>()
                .Configure(configure);
            services
                .AddHttpClient<IGameProvidersOperatorPluggableClient, GameProvidersOperatorPluggableClient>()
                .ConfigureHttpClient((sp, httpClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<GameProvidersPluggableClientOptions>>().Value;
                    httpClient.BaseAddress = options.BaseUrl;
                });
            services
            .AddHttpClient<IGameProvidersBackofficePluggableClient, GameProvidersBackofficePluggableClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameProvidersPluggableClientOptions>>().Value;

                httpClient.BaseAddress = options.BaseUrl;
            });

            return services;
        }
    }
}
