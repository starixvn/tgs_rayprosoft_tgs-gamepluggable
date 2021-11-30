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
        public void Get_GameProviderCode_Currencies_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            var actual = fakeRepository.GetCurrenciesAsync(Arg.Any<string>());

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Get_GameProviderCode_Currency_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            _ = await fakeRepository.GetCurrencyAsync(
                Arg.Any<string>(),
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
        public void Get_GameProviderId_Currencies_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            var actual = fakeRepository.GetCurrenciesAsync(Arg.Any<Guid>());

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Get_GameProviderId_Currency_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            _ = await fakeRepository.GetCurrencyAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>()
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

        [Test]
        public async Task Get_Sub_GameProvider_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderRepository>();
            _ = await fakeRepository.GetSubGameProviderAsync(
                Arg.Any<string>()
                ).ConfigureAwait(false);
        }
    }
}
