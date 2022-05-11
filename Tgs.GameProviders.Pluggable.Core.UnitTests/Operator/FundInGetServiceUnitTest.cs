using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class FundInGetServiceUnitTest
    {
        [Test]
        public async Task Fund_In()
        {
            var faceLogger = NullLogger<FundInGetService>.Instance;
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameProviderService = Substitute.For<IGameProviderService>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakeGameVerificationGrpc = Substitute.For<IGameVerificationGrpc>();

            var query = new FundInQuery(
                GameProviderType.KY,
                "4ucmqplxb029",
                1
                );

            var player = new PlayerInfo(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                (QueryServer.Models.PlayerStatus)PlayerStatus.Enabled);

            var gameProvider = new GameProviderInfo(
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 Tgs.GameProviders.QueryServer.Models.GameProviderStatus.Active,
                 string.Empty,
                 true,
                 true,
                 false,
                 0,
                 0,
                 (int)GameProviderUnique.KY,
                 true,
                 GameProviderType.KY,
                 GameProviderUnique.KY.ToString(),
                 string.Empty
                );

            decimal? gameProviderBalance = 10.0m;
            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            var gameProviderTransactionId = $"72412{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)):yyyyMMddHHmmssfff}4udwnknl3eg7";

            var transactionResponse = new TransactionResponse()
            {
                TransactionId = Guid.NewGuid(),
                Balance = 10
            };

            _ = fakePlayerService.GetAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameProviderService.GetAsync(
                Arg.Is(query.GameProviderCode)).Returns(ValueTask.FromResult(gameProvider));

            _ = fakeGameVerificationGrpc.VerifyPlayerFund(
                Arg.Is(player.PlayerCode), Arg.Is(gameProvider.GameProviderId)).Returns(true);

            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(player.PlayerCode)).Returns(ValueTask.FromResult(gameProviderBalance));

            _ = fakeGameProviderFundTransferTransactionRepository.AddFundInAsync(
                Arg.Is(gameProvider.GameProviderId),
                Arg.Is(player.DefaultWalletId),
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProvider.GameProviderCode),
                Arg.Is(query.Amount),
                Arg.Is(string.Empty)).Returns(ValueTask.FromResult(gameProviderFundTransferTransactionId));

            _ = fakeGameProvidersClient.PlayerFundInAsync(
                Arg.Is(gameProvider.GameProviderCode),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(gameProviderFundTransferTransactionId.ToString("N")),
                Arg.Is(query.Amount)).Returns(ValueTask.FromResult(gameProviderTransactionId));

            _ = fakeCashServiceClient.GameProviderFundInAsync(
                 Arg.Is(player.DefaultWalletId),
                 Arg.Is(query.Amount),
                 Arg.Is($"【{ gameProvider.GameProviderCode}】->【Default】"),
                 Arg.Is(gameProviderFundTransferTransactionId.ToString())).Returns(ValueTask.FromResult(transactionResponse));

            await fakeGameProviderFundTransferTransactionRepository.UpdateBySuccessAsync(
                 Arg.Is(gameProviderFundTransferTransactionId),
                 Arg.Is(transactionResponse.TransactionId),
                 Arg.Is(gameProviderTransactionId),
                 Arg.Is(transactionResponse.Balance),
                 Arg.Is(string.Empty));

            var fakeService = new FundInGetService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProvidersClient,
                fakeGameProviderService,
                fakeGameVerificationGrpc,
                faceLogger,
                fakePlayerService);

            _ = await fakeService.GetAsync(query).ConfigureAwait(false);
        }
    }
}
