using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

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
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();

            var fakeService = new FundInGetService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProviderRepository,
                fakeGameProvidersClient,
                faceLogger,
                fakePlayerRepository,
                fakePlayerVerifyService);

            var query = new FundInQuery(
                GameProviderType.KY,
                "4ucmqplxb029",
                1
                );

            var playerModel = new PlayerModel(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled
                );

            var gameProviderModel = new GameProviderModel(
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 GameProviderStatus.Active,
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

            decimal? playerBalance = 10.0m;
            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            var gameProviderTransactionId = $"72412{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).ToString("yyyyMMddHHmmssfff")}4udwnknl3eg7";

            var transactionResponse = new TransactionResponse()
            {
                TransactionId = Guid.NewGuid(),
                Balance = 10
            };

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(
                Arg.Is(query.PlayerCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository.GetAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(playerModel));

            _ = fakePlayerVerifyService.IsPlayerGameProviderActiveAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(query.GameProviderCode)).Returns(ValueTask.FromResult(true));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(query.GameProviderCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult(true));

            _ = fakeGameProviderRepository.GetAsync(
                Arg.Is(query.GameProviderCode)).Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakeGameProviderRepository.GetAsync(
                Arg.Is(query.GameProviderCode)).Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                query.GameProviderCode,
                playerModel.PlayerCode).Returns(ValueTask.FromResult(playerBalance));

            _ = fakeGameProviderFundTransferTransactionRepository.AddFundInAsync(
                Arg.Is(gameProviderModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletId),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(query.Amount),
                Arg.Is(string.Empty)).Returns(ValueTask.FromResult(gameProviderFundTransferTransactionId));

            _ = fakeGameProvidersClient.PlayerFundInAsync(
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(playerModel.BrandCode),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode),
                Arg.Is(gameProviderFundTransferTransactionId.ToString("N")),
                Arg.Is(query.Amount)).Returns(ValueTask.FromResult(gameProviderTransactionId));

            _ = fakeCashServiceClient.GameProviderFundInAsync(
                 Arg.Is(playerModel.DefaultWalletId),
                 Arg.Is(query.Amount),
                 Arg.Is($"【{ gameProviderModel.GameProviderCode}】->【Default】"),
                 Arg.Is(gameProviderFundTransferTransactionId.ToString())).Returns(ValueTask.FromResult(transactionResponse));

            await fakeGameProviderFundTransferTransactionRepository.UpdateBySuccessAsync(
                 Arg.Is(gameProviderFundTransferTransactionId),
                 Arg.Is(transactionResponse.TransactionId),
                 Arg.Is(gameProviderTransactionId),
                 Arg.Is(transactionResponse.Balance),
                 Arg.Is(string.Empty));

            _ = await fakeService.GetAsync(query).ConfigureAwait(false);
        }
    }
}
