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
    internal class GameProvidersPluggableClient : IGameProvidersPluggableClient
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

        public GameProvidersPluggableClient(HttpClient httpClient, JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId)
        {
            var response = await _httpClient
               .GetAsync(
                $"Round/{roundId}/GameActionQuery")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var payments = JsonSerializer.Deserialize<GameActionViewModel[]>(contentJson, _SerializerOptions);

            foreach (var payment in payments!)
                yield return payment;
        }

        public async ValueTask GameRoundClosedAsync(Guid roundId, GameRoundClosedViewModel viewModel)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(viewModel, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                    $"Round/{roundId}/GameRoundClosed"
                   , requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId)
        {
            var response = await _httpClient
               .GetAsync(
                $"Round/{roundId}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRoundViewModel>(contentJson, _SerializerOptions);
        }

        public async ValueTask KickPlayerAsync(string playerCode)
        {
            var response = await _httpClient
               .GetAsync(
                $"kickplayer/{playerCode}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask KickPlayerAsync(string playerCode, string gameProviderCode)
        {
            var response = await _httpClient
               .GetAsync(
                $"kickplayer/{playerCode}/{gameProviderCode}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}
