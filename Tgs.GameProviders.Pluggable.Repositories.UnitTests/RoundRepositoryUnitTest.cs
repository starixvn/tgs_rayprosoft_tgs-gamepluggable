using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class RoundRepositoryUnitTest
    {
        [Test]
        public async Task ClosedGameRoundAsync()
        {
            var fakeRepository = Substitute.For<IRoundRepository>();
            await fakeRepository.ClosedGameRoundAsync(
               Arg.Any<Guid>(),
               Arg.Any<decimal>(),
               Arg.Any<decimal>(),
               Arg.Any<decimal>(),
               Arg.Any<decimal>(),
               Arg.Any<decimal>(),
               Arg.Any<decimal>(),
               Arg.Any<ClientRoundStatus>(),
               Arg.Any<DateTimeOffset>()
               ).ConfigureAwait(false);
        }

        [Test]
        public async Task GetAsync()
        {
            var fakeRepository = Substitute.For<IRoundRepository>();
            await fakeRepository.GetAsync(
               Arg.Any<Guid>()
               ).ConfigureAwait(false);
        }
    }
}
