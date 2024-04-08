using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class GameUrlGetService(
	IGame game,
	IGameUrl gameUrl,
	IVerification verification,
	IPlayer player) : IGetService<GameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await player.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameInfo = await game.GetAsync(
			gameId: query.GameId,
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
					playerInfo.DefaultWalletCurrencyCode,
					query.LoginRecordId,
					query.PlatformType,
					query.Language,
					query.IpAddress,
					query.BackUrl,
					playerInfo.DefaultWalletId,
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
					playerInfo.DefaultWalletCurrencyCode,
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
					playerInfo.DefaultWalletCurrencyCode,
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
