using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class PlayerRepositoryUnitTest
    {
        [Test]
        public async Task GetAsync()
        {
            var fakeRepository = Substitute.For<IPlayerRepository>();
            _ = await fakeRepository.GetAsync(
                Arg.Any<string>()
                ).ConfigureAwait(false);
        }
    }
}
