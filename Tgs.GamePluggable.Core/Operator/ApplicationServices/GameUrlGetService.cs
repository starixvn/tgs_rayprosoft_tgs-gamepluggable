using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class GameUrlGetService(
	IGameRepository gameRepository,
	IGameUrl gameUrl,
	IVerification verification,
	IPlayerRepository playerRepository) : IGetService<GameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameInfo = await gameRepository.GetAsync(
			id: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await verification.VerifyPlayerGameUrlAsync(
			player: playerInfo,
			game: gameInfo,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return gameInfo.GameProviderCode switch
		{
			var o when o == GameProviderType.AsiaGaming
			=> await gameUrl
				.GetGameUrlByAsiaGamingAsync(
					gameInfo.GameProviderCode,
					(GameProvidersClientGameType)gameInfo.Type,
					gameInfo.GameId,
					playerInfo.BrandCode,
					playerInfo.PlayerCode,
					playerInfo.CurrencyCode,
					query.LoginRecordId,
					query.PlatformType,
					query.Language,
					query.IpAddress,
					query.BackUrl,
					playerInfo.WalletId,
					cancellationToken)
				.ConfigureAwait(false),
			var o when o == GameProviderType.BetConstruct
			=> await gameUrl
				.GetGameUrlByBetConstructAsync(
					gameInfo.GameProviderCode,
					(GameProvidersClientGameType)gameInfo.Type,
					gameInfo.GameId,
					playerInfo.BrandCode,
					playerInfo.PlayerCode,
					playerInfo.CurrencyCode,
					query.LoginRecordId,
					query.PlatformType,
					query.Language,
					query.IpAddress,
					query.BackUrl,
					playerInfo.BrandId,
					gameInfo.GameProviderId,
					cancellationToken)
				.ConfigureAwait(false),
			_ => await gameUrl
				.GetGameUrlAsync(
					gameInfo.GameProviderCode,
					(GameProvidersClientGameType)gameInfo.Type,
					gameInfo.GameId,
					playerInfo.BrandCode,
					playerInfo.PlayerCode,
					playerInfo.CurrencyCode,
					query.LoginRecordId,
					query.PlatformType,
					query.Language,
					query.IpAddress,
					query.BackUrl,
					cancellationToken)
				.ConfigureAwait(false)
		};
	}
}
