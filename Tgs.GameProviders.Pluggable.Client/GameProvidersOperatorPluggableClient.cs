using System;
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

        public async ValueTask<GameProviderFundResultViewModel?> GameProviderFundInAsync(string playerCode, string gameProviderCode, decimal amount)
        {
            try
            {
                var request = new
                {
                    Amount = amount
                };
                var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

                var response = await _httpClient
                .PostAsync(
                $"/op/player/fundin/{playerCode}/{gameProviderCode}",
                requestContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonSerializer.Deserialize<GameProviderFundResultViewModel?>(contentJson, _SerializerOptions);
            }
            catch
            {
                return null;
            }
        }

        public async ValueTask<GameProviderFundResultViewModel?> GameProviderFundOutAsync(string playerCode, string gameProviderCode, decimal amount)
        {
            try
            {
                var request = new
                {
                    Amount = amount
                };
                var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

                var response = await _httpClient
                .PostAsync(
                $"/op/player/fundout/{playerCode}/{gameProviderCode}",
                requestContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonSerializer.Deserialize<GameProviderFundResultViewModel?>(contentJson, _SerializerOptions);
            }
            catch
            {
                return null;
            }
        }

        public async ValueTask<decimal?> GetBalanceAsync(string playerCode, string gameProviderCode)
        {
            try
            {
                var response = await _httpClient
                .GetAsync(
                $"/op/player/balance/{playerCode}/{gameProviderCode}").ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonSerializer.Deserialize<decimal?>(contentJson, _SerializerOptions);
            }
            catch
            {
                return null;
            }
        }

        public async ValueTask<Uri?> GetGameUrlAsync(string playerCode, Guid loginRecordId, bool isMobile, int gameId, string language, string ipAddress, Uri? backUrl)
        {
            try
            {
                var request = new
                {
                    PlayerCode = playerCode,
                    LoginRecordId = loginRecordId,
                    IsMobile = isMobile,
                    GameId = gameId,
                    Language = language,
                    IpAddress = ipAddress,
                    BackUrl = backUrl
                };
                var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

                var response = await _httpClient
                .PostAsync(
                    $"/op/player/gameurl"
                    , requestContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonSerializer.Deserialize<Uri?>(contentJson, _SerializerOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}
