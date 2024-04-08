using System.Threading;
using System.Threading.Tasks;
using Tgs.GamePlayerLogOut.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class PlayerLogOutCommandService(IGamePlayerLogOutClient client) : ICommandService<PlayerLogOutCommand>
{
	public ValueTask ExecuteAsync(PlayerLogOutCommand command, CancellationToken cancellationToken = default)
		=> client.LogOutAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken);
}
