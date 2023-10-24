using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class PlayerRepositoryUnitTest
{
    [Fact]
    public async Task Get()
    {
        var fakeClinet = Substitute.For<IGameCommandClient>();

        var playerCode = "4ucmqplxb029";

        var player = new PlayerInfo(
            Guid.NewGuid(),
            "TGS",
            Guid.NewGuid(),
            "TGS",
            Guid.NewGuid(),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.NewGuid(),
            "CNY");
        _ = fakeClinet
            .PlayerGetAsync(
                playerCode: Arg.Is(playerCode))
            .Returns(player);

        var repository = new PlayerRepository(
            fakeClinet);

        var actual = await repository.GetAsync(playerCode);

        Assert.NotNull(actual);
    }
}
