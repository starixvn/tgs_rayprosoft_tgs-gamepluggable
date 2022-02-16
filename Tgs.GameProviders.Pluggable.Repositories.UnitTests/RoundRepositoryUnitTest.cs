using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTests
{
    [TestFixture]
    public class RoundRepositoryUnitTest
    {
        [Test]
        public async Task ClosedGameRoundAsync()
        {
            var fakeRepository = Substitute.For<IRoundRepository>();
            var roundId = Guid.NewGuid();

            decimal betAmount = 100;
            decimal validBetAmount = 100;
            decimal settleAmount = 0;
            decimal commission = 0;
            decimal turnover = 100;
            decimal winLoss = 100;
            RoundClientStatus clientRoundStatus = RoundClientStatus.Loss;
            DateTimeOffset externalClosedOn = DateTimeOffset.UtcNow;
            var operatorId = Guid.NewGuid();
            string remark = "測試關閉遊戲";
            await fakeRepository.ClosedGameRoundAsync(
               Arg.Is(roundId),
               Arg.Is(betAmount),
               Arg.Is(validBetAmount),
               Arg.Is(settleAmount),
               Arg.Is(commission),
               Arg.Is(turnover),
               Arg.Is(winLoss),
               Arg.Is(clientRoundStatus),
               Arg.Is(externalClosedOn),
               Arg.Is(operatorId),
               Arg.Is(remark)
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
