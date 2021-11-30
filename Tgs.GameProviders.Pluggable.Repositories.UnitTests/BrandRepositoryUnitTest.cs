using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class BrandRepositoryUnitTest
    {
        [Test]
        public void Get_Currencies_Async()
        {
            var fakeRepository = Substitute.For<IBrandRepository>();
            var actual = fakeRepository.GetCurrenciesAsync(
                Arg.Any<Guid>());

            Assert.IsNotNull(actual);
        }

        [Test]
        public void Get_Currency_Async()
        {
            var fakeRepository = Substitute.For<IBrandRepository>();
            var actual = fakeRepository.GetCurrencyAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>());

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Get_GameProviderStatus_Async()
        {
            var fakeRepository = Substitute.For<IBrandRepository>();
            _ = await fakeRepository.GetGameProviderStatusAsync(
                Arg.Any<Guid>(),
                Arg.Any<Guid>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Get_GameStatus_Async()
        {
            var fakeRepository = Substitute.For<IBrandRepository>();
            _ = await fakeRepository.GetGameStatusAsync(
                Arg.Any<Guid>(),
                Arg.Any<int>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Get_Status_Async()
        {
            var fakeRepository = Substitute.For<IBrandRepository>();
            _ = await fakeRepository.GetStatusAsync(
                Arg.Any<Guid>()).ConfigureAwait(false);
        }
    }
}
