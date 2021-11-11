using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client
{
    internal class GameProvidersPluggableClient : IGameProvidersPluggableClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _serializerOptions;

        public GameProvidersPluggableClient(HttpClient httpClient, JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = serializerOptions;
        }

        public async ValueTask ClosedGameRoundAsync(Guid roundId, ClosedGameRoundViewModel viewModel)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(viewModel, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                    $"Round/{roundId}/ClosedGameRound"
                   , requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}
