using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator
{
    [TestFixture]
    public class FundInGetServiceUnitTest
    {
        [Test]
        public async Task Fund_In()
        {
            var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
            var fakeGameProvider = Substitute.For<IGameProvider>();
            var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
            var fakePlayer = Substitute.For<IPlayer>();
            var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

            var query = new FundInCommand(
                "4ucmqplxb029",
                GameProviderType.KY,
                1);

            var player = new PlayerInfo(
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY");
            _ = fakePlayer
                .GetAsync(
                    Arg.Is(query.PlayerCode))
                .Returns(ValueTask.FromResult(player));

            var gameProviderId = Guid.NewGuid();
            _ = fakeGameProvider
                .GetGameProviderIdAsync(
                    Arg.Is(query.GameProviderCode))
                .Returns(ValueTask.FromResult(gameProviderId));

            var gameProvider = new GameProviderInfo(
                gameProviderId,
                GameProviderType.KY);
            _ = fakeGameProvider
                .GetAsync(
                    Arg.Is(gameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            _ = fakeGameVerificationClient
                .VerifyPlayerFundAsync(
                    Arg.Is(player.PlayerCode),
                    Arg.Is(gameProvider.GameProviderId))
                .Returns(true);

            decimal gameProviderBalance = 10.0m;
            _ = fakePlayerGameBalance
                .GetBalanceAsync(
                    Arg.Is(query.GameProviderCode),
                    Arg.Is(player.PlayerCode),
                    Arg.Is(player.DefaultWalletCurrencyCode),
                    Arg.Any<CancellationToken>())
                .Returns(ValueTask.FromResult(gameProviderBalance));

            var fakeService = new FundInCommandService(
                fakeGameFundTransferClient,
                fakeGameProvider,
                fakeGameVerificationClient,
                fakePlayer,
                fakePlayerGameBalance);

            await fakeService
                .ExecuteAsync(query)
                .ConfigureAwait(false);

            _ = fakeGameFundTransferClient
                .Received(1)
                .FundInAsync(Arg.Any<GameFundTransferFundRequest>());
        }
    }
}
