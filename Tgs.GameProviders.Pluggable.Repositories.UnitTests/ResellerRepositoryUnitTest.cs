using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class ResellerRepositoryUnitTest
    {
        [Test]
        public async Task Get_Status_Async()
        {
            var fakeRepository = Substitute.For<IResellerRepository>();
            _ = await fakeRepository.GetStatusAsync(
                Arg.Any<Guid>()
                ).ConfigureAwait(false);
        }
    }
}
