using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    internal class GameProvidersOperatorPluggableClient : IGameProvidersOperatorPluggableClient
    {
        private static readonly JsonSerializerOptions? _SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        private readonly HttpClient _httpClient;

        public GameProvidersOperatorPluggableClient(HttpClient httpClient)
            => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode)
        {
            try
            {
                var response = await _httpClient
                .GetAsync(
                $"/op/player/balance/{playerCode}/{gameProviderCode}")
                .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonSerializer.Deserialize<decimal?>(contentJson, _SerializerOptions);

            }
            catch
            {
                return null;
            }
        }

    }
}
