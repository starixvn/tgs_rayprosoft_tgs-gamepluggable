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
    internal class GameProvidersBackofficePluggableClient : IGameProvidersBackofficePluggableClient
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

        public GameProvidersBackofficePluggableClient(HttpClient httpClient)
            => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId)
        {
            var response = await _httpClient
               .GetAsync(
                $"/bo/round/{roundId}/gameactionquery")
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
                    $"/bo/round/{roundId}/gameroundclosed"
                   , requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask AdjustClosedRoundAsync(Guid roundId, AdjustClosedRoundViewModel viewModel)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(viewModel, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                    $"/bo/round/{roundId}/adjustclosedRound"
                   , requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask<JsonDocument?> GameRoundDetailGetAsync(Guid roundId, string language)
        {
            var response = await _httpClient
               .GetAsync(
                $"/bo/round/{roundId}/gamerounddetail?language={language}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<JsonDocument>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId)
        {
            var response = await _httpClient
               .GetAsync(
                $"/bo/round/{roundId}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRoundViewModel>(contentJson, _SerializerOptions);
        }

        public async ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode)
        {
            try
            {
                var response = await _httpClient
                .GetAsync(
                $"/bo/player/balance/{playerCode}/{gameProviderCode}")
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

        public Task KickPlayerAsync(string playerCode)
            => _httpClient
                .DeleteAsync($"/bo/player/kick/{playerCode}");

        public Task KickPlayerAsync(string playerCode, string gameProviderCode)
            => _httpClient
                .DeleteAsync($"/bo/player/kick/{playerCode}/{gameProviderCode}");
    }
}