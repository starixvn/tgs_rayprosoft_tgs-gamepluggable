using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class RoundOperationRecordsRepositoryUnitTest
    {
        [Test]
        public async Task Add_Operation_Record_Async()
        {
            var fakeRepository = Substitute.For<IRoundOperationRecordsRepository>();
            await fakeRepository.AddAsync(
               Arg.Any<Guid>(),
               Arg.Any<Guid>(),
               Arg.Any<string>(),
               Arg.Any<string>(),
               Arg.Any<string>()
               ).ConfigureAwait(false);
        }
    }
}
