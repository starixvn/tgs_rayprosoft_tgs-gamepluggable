using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class PlayerRepository(
	IGameCommandClient client) : IPlayer
{
	public async ValueTask<PlayerInfo> GetAsync(
		string playerCode,
		CancellationToken cancellationToken = default)
		=> await client.PlayerGetAsync(
			playerCode: playerCode,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new Exception($"This player does not exist.({playerCode})");
}
