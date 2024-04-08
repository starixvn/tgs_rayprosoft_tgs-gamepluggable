using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GamePlayerLogOut.Client;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class PlayerLogOutGameProviderCommandServiceUnitTest
{
	[Fact]
	public async Task Call_LogOut()
	{
		var fakeClient = Substitute.For<IGamePlayerLogOutClient>();

		var command = new PlayerLogOutGameProviderCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var service = new PlayerLogOutGameProviderCommandService(
			fakeClient);

		await service.ExecuteAsync(command);

		_ = fakeClient
			.Received(1)
			.LogOutAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
