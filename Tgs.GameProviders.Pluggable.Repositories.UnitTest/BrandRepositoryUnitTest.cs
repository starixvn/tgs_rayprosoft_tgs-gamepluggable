using NSubstitute;
using Tgs.GameCommand.Client;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

[TestClass]
public class BrandRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_GameProvider_ConfigurationId()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeClient
            .BrandGetGameProviderConfigurationIdAsync(
                brandId: Arg.Is(brandId),
                gameProviderId: Arg.Is(gameProviderId))
            .Returns(gameProviderConfigurationId);

        var repository = new BrandRepository(
            fakeClient);

        var actual = await repository
           .GetGameProviderConfigurationIdAsync(
                brandId: brandId,
                gameProviderId: gameProviderId)
           .ConfigureAwait(false);

        Assert.IsNotNull(actual);
    }
}
