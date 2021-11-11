using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class GameProviderRepositoryUnitTest
    {
        [Test]
        public async Task Get_GameProviderCode_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            _ = await fakeRepository.GetAsync(
                Arg.Any<string>()
                ).ConfigureAwait(false);
        }

        [Test]
        public async Task Get_GameProviderId_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            _ = await fakeRepository.GetAsync(
                Arg.Any<Guid>()
                ).ConfigureAwait(false);
        }

        [Test]
        public async Task Get_Status_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            _ = await fakeRepository.GetStatusAsync(
                Arg.Any<Guid>()
                ).ConfigureAwait(false);
        }
    }
}
