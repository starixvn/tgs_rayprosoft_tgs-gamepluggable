using System;
using System.Threading;
using System.Threading.Tasks;
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

		return await gameUrl.GetGameUrlAsync(
			gameProviderCode: gameInfo.GameProviderCode,
			playerCode: playerInfo.PlayerCode,
			gameId: gameInfo.GameId,
			loginRecordId: query.LoginRecordId,
			language: query.Language,
			platformType: query.PlatformType,
			ipAddress: query.IpAddress,
			backUrl: query.BackUrl,
			extraInfo: query.ExtraInfo,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
