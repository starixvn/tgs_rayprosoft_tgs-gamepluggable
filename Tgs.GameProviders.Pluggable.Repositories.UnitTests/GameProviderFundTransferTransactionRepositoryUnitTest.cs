using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class GameProviderFundTransferTransactionRepositoryUnitTest
    {
        [Test]
        public async Task Add_FundIn_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            _ = await fakeRepository.AddFundInAsync(
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<string>()
                ).ConfigureAwait(false);
        }

        [Test]
        public async Task Add_FundOut_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            _ = await fakeRepository.AddFundOutAsync(
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<string>()
                ).ConfigureAwait(false);
        }

        [Test]
        public async Task Update_By_FailureOnGameProvider_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            await fakeRepository.UpdateByFailureOnGameProviderAsync(
               Arg.Any<Guid>(),
               Arg.Any<Guid>(),
               Arg.Any<string>(),
               Arg.Any<decimal>(),
               Arg.Any<string>()
               ).ConfigureAwait(false);
        }

        [Test]
        public async Task Update_By_FailureOnMainWallet_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            await fakeRepository.UpdateByFailureOnMainWalletAsync(
               Arg.Any<Guid>(),
               Arg.Any<Guid>(),
               Arg.Any<string>(),
               Arg.Any<decimal>(),
               Arg.Any<string>()
               ).ConfigureAwait(false);
        }

        [Test]
        public async Task Update_By_Success_Async()
        {
            var fakeRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            await fakeRepository.UpdateBySuccessAsync(
               Arg.Any<Guid>(),
               Arg.Any<Guid>(),
               Arg.Any<string>(),
               Arg.Any<decimal>(),
               Arg.Any<string>()
               ).ConfigureAwait(false);
        }
    }
}
