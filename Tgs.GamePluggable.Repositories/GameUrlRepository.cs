using Tgs.CashService.Client;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Client;

namespace Tgs.GamePluggable.Repositories;

internal class GameUrlRepository(
	IBrandRepository brand,
	ICashServiceClient cashServiceClient,
	IGameProviderRepository gameProvider,
	IGameProvidersClient gameProvidersClient) : IGameUrl
{
	public async ValueTask<GameSM01PlatformLoginInfo> GetGamePlatformLoginBySM01Async(string playerCode, CancellationToken cancellationToken = default)
	{
		var result = await gameProvidersClient
			.GamePlatformLoginBySM01Async(
				playerCode,
				cancellationToken)
			.ConfigureAwait(false);

		return new GameSM01PlatformLoginInfo(result!.ApiUrl, result.MerchantId, result.CustomerName, result.Timestamp, result.Sign);
	}

	public async ValueTask<Uri> GetGameUrlAsync(string gameProviderCode, GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, CancellationToken cancellationToken = default)
		=> await gameProvidersClient
			.GameUrlAsync(
				gameProviderCode,
				gameType,
				gameId,
				brandCode,
				playerCode,
				currencyCode,
				loginRecordId,
				platformType,
				language,
				ipAddress,
				backUrl,
				cancellationToken)
		.ConfigureAwait(false)
		?? throw new NullReferenceException();

	public async ValueTask<Uri> GetGameUrlByAsiaGamingAsync(string gameProviderCode, GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, Guid defaultWalletId, CancellationToken cancellationToken = default)
	{
		var playerBalance = await cashServiceClient
				.GetBalanceAsync(
					defaultWalletId,
					cancellationToken)
				.ConfigureAwait(false);

		return await gameProvidersClient
				.GameUrlByAsiaGamingAsync(
					gameType,
					gameId,
					brandCode,
					playerCode,
					currencyCode,
					loginRecordId,
					platformType,
					language,
					ipAddress,
					backUrl,
					playerBalance,
					cancellationToken)
				.ConfigureAwait(false)
				?? throw new NullReferenceException();
	}

	public async ValueTask<Uri> GetGameUrlByBetConstructAsync(string gameProviderCode, GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default)
	{
		var gameProviderConfigurationId = await brand
			.GetGameProviderConfigurationIdAsync(
				brandId,
				gameProviderId,
				cancellationToken)
			.ConfigureAwait(false);

		var gameProviderConfiguration = await gameProvider
			.GetConfigurationAsync(
				gameProviderConfigurationId,
				cancellationToken)
			.ConfigureAwait(false);

		var lobbyDomain = platformType switch
		{
			GameProvidersClientPlatformType.Desktop => gameProviderConfiguration.DesktopEndpoint,
			GameProvidersClientPlatformType.Mobile => gameProviderConfiguration.MobileEndpoint,
			_ => gameProviderConfiguration.DesktopEndpoint
		};

		return await gameProvidersClient
			.GameUrlByBetConstructAsync(
				gameType,
				gameId,
				brandCode,
				playerCode,
				currencyCode,
				loginRecordId,
				platformType,
				language,
				ipAddress,
				backUrl,
				lobbyDomain,
				cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();
	}
}
