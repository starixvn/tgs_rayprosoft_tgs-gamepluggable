using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Client;

internal class GameProvidersOperatorPluggableClient(HttpClient httpClient) : IGameProvidersOperatorPluggableClient
{
	private static readonly JsonSerializerOptions? _SerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters =
		{
			new JsonStringEnumConverter()
		}
	};

	public async ValueTask GameProviderFundInAsync(string playerCode, string gameProviderCode, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new
		{
			Amount = amount
		};
		var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
		requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

		var response = await httpClient
			.PostAsync(
				requestUri: $"/op/player/fundin/{playerCode}/{gameProviderCode}",
				content: requestContent,
				cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask GameProviderFundOutAsync(string playerCode, string gameProviderCode, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new
		{
			Amount = amount
		};
		var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
		requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

		var response = await httpClient
			.PostAsync(
				requestUri: $"/op/player/fundout/{playerCode}/{gameProviderCode}",
				content: requestContent,
				cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<decimal> GetBalanceAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient
			.GetAsync(
				requestUri: $"/op/player/balance/{playerCode}/{gameProviderCode}",
				cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

		return JsonSerializer.Deserialize<decimal>(contentJson, _SerializerOptions)!;
	}

	public async ValueTask<SM01PlatformLoginResponse> GetGameSM01PlatformLoginInfoAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new
		{
			PlayerCode = playerCode,
		};
		var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
		requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

		var response = await httpClient
			.PostAsync(
				requestUri: "/op/player/gameurl/sm01",
				content: requestContent,
				cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

		return JsonSerializer.Deserialize<SM01PlatformLoginResponse>(contentJson, _SerializerOptions)!;
	}

	public async ValueTask<Uri> GetGameUrlAsync(string playerCode, Guid loginRecordId, bool isMobile, int gameId, string language, string ipAddress, Uri? backUrl = null, CancellationToken cancellationToken = default)
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

		var response = await httpClient
			.PostAsync(
				requestUri: "/op/player/gameurl",
				content: requestContent,
				cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		var contentJson = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

		return JsonSerializer.Deserialize<Uri>(contentJson, _SerializerOptions)!;
	}
}
