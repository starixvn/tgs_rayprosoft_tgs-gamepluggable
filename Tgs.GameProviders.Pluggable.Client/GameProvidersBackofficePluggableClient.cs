using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
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

        public async ValueTask AddRoundAsync(AddRoundViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(viewModel, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   requestUri: $"/bo/round/addRound",
                   content: requestContent,
                   cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async ValueTask AdjustClosedRoundAsync(Guid roundId, AdjustClosedRoundViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(viewModel, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   requestUri: $"/bo/round/{roundId}/adjustclosedRound",
                   content: requestContent,
                   cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async ValueTask FundInAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                playerCode,
                gameProviderCode
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   requestUri: "/bo/player/fund/in",
                   content: requestContent,
                   cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
               .GetAsync(
                    requestUri: $"/bo/round/{roundId}/gameactionquery",
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var payments = JsonSerializer.Deserialize<GameActionViewModel[]>(contentJson, _SerializerOptions);

            if (payments == null || !payments.Any())
            {
                yield break;
            }

            foreach (var payment in payments!)
                yield return payment;
        }

        public async ValueTask GameRoundClosedAsync(Guid roundId, GameRoundClosedViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(viewModel, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   requestUri: $"/bo/round/{roundId}/gameroundclosed",
                   content: requestContent,
                   cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async ValueTask<JsonDocument?> GameRoundDetailGetAsync(Guid roundId, string language, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
               .GetAsync(
                    requestUri: $"/bo/round/{roundId}/gamerounddetail?language={language}",
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return JsonSerializer.Deserialize<JsonDocument>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
               .GetAsync(
                    requestUri: $"/bo/round/{roundId}",
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRoundViewModel>(contentJson, _SerializerOptions);
        }

        public async ValueTask<decimal> GetBalanceAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
            .GetAsync(
                    requestUri: $"/bo/player/balance/{playerCode}/{gameProviderCode}",
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return JsonSerializer.Deserialize<decimal>(contentJson, _SerializerOptions);
        }

        public Task KickPlayerAsync(string playerCode, CancellationToken cancellationToken = default)
            => _httpClient
                .DeleteAsync(
                    requestUri: $"/bo/player/kick/{playerCode}",
                    cancellationToken: cancellationToken);

        public Task KickPlayerAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default)
            => _httpClient
                .DeleteAsync(
                    requestUri: $"/bo/player/kick/{playerCode}/{gameProviderCode}",
                    cancellationToken: cancellationToken);
    }
}
