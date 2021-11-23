using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GameProvidersPluggableClient> _logger;

        public GameProvidersPluggableClient(
            HttpClient httpClient,
            ILogger<GameProvidersPluggableClient> _logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId)
        {
            var response = await _httpClient
               .GetAsync(
                $"round/{roundId}/gameactionquery")
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
                    $"/round/{roundId}/gameroundclosed"
                   , requestContent)
               .ConfigureAwait(false);

            _logger.LogInformation(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId)
        {
            var response = await _httpClient
               .GetAsync(
                $"/round/{roundId}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRoundViewModel>(contentJson, _SerializerOptions);
        }

        public async ValueTask KickPlayerAsync(string playerCode)
        {
            var response = await _httpClient
               .DeleteAsync(
                $"/player/kickplayer/{playerCode}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask KickPlayerAsync(string playerCode, string gameProviderCode)
        {
            var response = await _httpClient
               .DeleteAsync(
                $"/player/kickplayer/{playerCode}/{gameProviderCode}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}
