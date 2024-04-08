using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class GameSM01PlatformLoginGetService(
	IGameUrl gameUrl,
	IVerification verification,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IGameRepository gameRepository) : IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>
{
	public async ValueTask<GameSM01PlatformLoginInfo> GetAsync(GameSM01PlatformLoginQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository
			.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProviderRepository
			.GetGameProviderIdAsync(
			code: GameProviderType.SM01,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameIds = await gameRepository.QueryAsync(
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<int>();

		var gameId = gameIds.ElementAt(0);

		var game = await gameRepository.GetAsync(
			gameId,
			cancellationToken)
			.ConfigureAwait(false);

		await verification
			.VerifyPlayerGameUrlAsync(
			player: playerInfo,
			game: game,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var info = await gameUrl
			.GetGamePlatformLoginBySM01Async(
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameSM01PlatformLoginInfo(info.ApiUrl, info.MerchantId, info.CustomerName, info.Timestamp, info.Sign);
	}
}
