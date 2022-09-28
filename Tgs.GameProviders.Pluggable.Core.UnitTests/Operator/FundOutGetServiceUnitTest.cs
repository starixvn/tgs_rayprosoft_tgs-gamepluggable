using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class FundOutGetServiceUnitTest
    {
        [Test]
        public async Task Fund_Out()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
            var fakeGameProviderService = Substitute.For<IGameProviderService>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakeGameVerification = Substitute.For<IGameVerificationClient>();

            var query = new FundOutQuery(
                GameProviderType.KY,
                "4ucmqplxb029",
                1
                );

            var player = new PlayerInfo(
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY");

            var gameProvider = new GameProviderInfo(
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                QueryServer.Models.GameProviderStatus.Active,
                 string.Empty,
                 true,
                 true,
                 false,
                 0,
                 0,
                 (int)10,
                 true,
                 GameProviderType.KY,
                 10.ToString(),
                 string.Empty
                 );

            var balance = 10.0m;

            _ = fakePlayerService.GetAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameProviderService.GetAsync(
                Arg.Is(query.GameProviderCode)).Returns(ValueTask.FromResult(gameProvider));

            _ = fakeGameVerification.VerifyPlayerFundAsync(
                Arg.Is(player.PlayerCode), Arg.Is(gameProvider.GameProviderId)).Returns(true);

            _ = fakeCashServiceClient.GetBalanceAsync(
                Arg.Is(player.DefaultWalletId)).Returns(ValueTask.FromResult(balance));

            var fakeService = new FundOutGetService(
                fakeCashServiceClient,
                fakeGameFundTransferClient,
                fakeGameProviderService,
                fakeGameVerification,
                fakePlayerService);

            await fakeService.ExecuteAsync(query).ConfigureAwait(false);

            var request = new GameFundTransferFundRequest()
            {
                GameProviderId = gameProvider.GameProviderId,
                GameProviderCode = gameProvider.GameProviderCode,
                BrandCode = player.BrandCode,
                PlayerCode = player.PlayerCode,
                CurrencyCode = player.DefaultWalletCurrencyCode,
                WalletId = player.DefaultWalletId,
                Amount = query.Amount
            };

            _ = fakeGameFundTransferClient.Received(1).FundOutAsync(request);
        }
    }
}
