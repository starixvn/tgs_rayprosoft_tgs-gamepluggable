using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GamePlayerLogOut.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Backoffice;

public class PlayerLogOutCommandServiceUnitTest
{
    [Fact]
    public async Task Call_LogOut()
    {
        var fakeClient = Substitute.For<IGamePlayerLogOutClient>();

        var command = new PlayerLogOutCommand(
             PlayerCode: "4ucmqplxb029");

        var service = new PlayerLogOutCommandService(
            fakeClient);

        await service.ExecuteAsync(command);

        _ = fakeClient
            .Received(1)
            .LogOutAsync(
            playerCode: Arg.Is(command.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
