using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class GameRepositoryUnitTest
    {
        [Test]
        public async Task Get_Game_Async()
        {
            var fakeRepository = Substitute.For<IGameRepository>();
            _ = await fakeRepository.GetAsync(
                Arg.Any<int>()
                ).ConfigureAwait(false);
        }

        [Test]
        public async Task Get_Status_Async()
        {
            var fakeRepository = Substitute.For<IGameRepository>();
            _ = await fakeRepository.GetStatusAsync(
                Arg.Any<int>()
                ).ConfigureAwait(false);
        }
    }
}
