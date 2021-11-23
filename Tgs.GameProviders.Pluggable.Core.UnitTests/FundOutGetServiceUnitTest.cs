using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Core.ApplicationServices;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests
{
    [TestFixture]
    public class FundOutGetServiceUnitTest
    {
        [Test]
        public async Task Fund_Out()
        {
            var faceLogger = NullLogger<FundOutGetService>.Instance;
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();

            var fakeService = new FundOutGetService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProviderRepository,
                fakeGameProvidersClient,
                faceLogger,
                fakePlayerRepository,
                fakePlayerVerifyService);

            var query = new FundOutQuery()
            {
                GameProviderCode = "ky",
                PlayerCode = "4ucmqplxb029",
                Amount = 1
            };

            var playerModel = new PlayerModel()
            {
                ResellerId = Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                LicenseeId = Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                LicenseeCode = "TGS",
                BrandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                BrandCode = "GPI",
                PlayerCode = "4ucmqplxb029",
                Username = "4ucmqplxb029",
                RealName = "4ucmqplxb029",
                DefaultWalletId = Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                DefaultWalletCurrencyCode = "CNY",
                Status = PlayerStatus.Enabled
            };

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

            var playerBalance = 0m;
            var balance = 10.0m;
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
                Arg.Is(query.PlayerCode)
                ).Returns(ValueTask.FromResult(playerModel));

            _ = fakePlayerVerifyService.IsPlayerGameProviderActiveAsync(
                Arg.Is(playerModel.BrandId), 
                Arg.Is(query.GameProviderCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakeGameProviderRepository.GetAsync(
                Arg.Is(query.GameProviderCode)
                ).Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(playerModel.PlayerCode))
                .Returns(ValueTask.FromResult(playerBalance));

            _ = fakeCashServiceClient.GetBalanceAsync(
                Arg.Is(playerModel.DefaultWalletId))
                .Returns(ValueTask.FromResult(balance));

            _ = fakeGameProviderFundTransferTransactionRepository.AddFundOutAsync(
                Arg.Is(gameProviderModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletId),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(query.Amount * -1),
                Arg.Is(string.Empty))
                .Returns(ValueTask.FromResult(gameProviderFundTransferTransactionId));

            _ = fakeCashServiceClient.GameProviderFundOutAsync(
                 Arg.Is(playerModel.DefaultWalletId),
                 Arg.Is(query.Amount),
                 Arg.Is($"【Default】->【{ gameProviderModel.GameProviderCode}】"),
                 Arg.Is(gameProviderFundTransferTransactionId.ToString()))
                 .Returns(ValueTask.FromResult(transactionResponse));

            _ = fakeGameProvidersClient.PlayerFundOutAsync(
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(playerModel.BrandCode),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode),
                Arg.Is(gameProviderFundTransferTransactionId.ToString("N")),
                Arg.Is(query.Amount))
                .Returns(ValueTask.FromResult(gameProviderTransactionId));

            await fakeGameProviderFundTransferTransactionRepository.UpdateFundInAsync(
                Arg.Is(gameProviderFundTransferTransactionId),
                Arg.Is(transactionResponse.TransactionId),
                Arg.Is(FundTransferTransactionStatus.Success),
                Arg.Is(gameProviderTransactionId),
                Arg.Is(transactionResponse.Balance),
                Arg.Is(string.Empty)
                );

            _ = await fakeService.GetAsync(query).ConfigureAwait(false);
        }
    }
}
