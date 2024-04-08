using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

internal class GameSM01PlatformLoginGetService(
	IGameUrl gameUrl,
	IVerification verification,
	IGameProvider gameProvider,
	IGameCommandClient gameCommandClient,
	IPlayer player) : IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo>
{
	public async ValueTask<GameSM01PlatformLoginInfo> GetAsync(GameSM01PlatformLoginQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await player
			.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderId = await gameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SM01,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var games = await gameCommandClient
			.GameQueryAsync(
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameInfo>();

		var game = games.ElementAt(0);

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
