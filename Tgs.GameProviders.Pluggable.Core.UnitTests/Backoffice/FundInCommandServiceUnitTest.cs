using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Backoffice
{
    [TestFixture]
    public class FundInCommandServiceUnitTest
    {
        [Test]
        public async Task Game_Provider_Balance_0_006_Transferred_To_Main_Wallet()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
            var fakeLogger = NullLogger<FundInCommandService>.Instance;
            var fakePlayerService = Substitute.For<IPlayerService>();

            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "KY";
            var defaultWalletId = Guid.NewGuid();

            var playerInfo = new PlayerInfo(
                ResellerId: Guid.NewGuid(),
                ResellerCode: "TGS",
                LicenseeId: Guid.NewGuid(),
                LicenseeCode: "TGS",
                BrandId: Guid.NewGuid(),
                BrandCode: "GPI",
                PlayerCode: playerCode,
                Username: playerCode,
                DefaultWalletId: defaultWalletId,
                DefaultWalletCurrencyCode: "CNY",
                Status: PlayerStatus.Enabled);
            _ = fakePlayerService.GetAsync(
                Arg.Is(playerCode)).Returns(playerInfo);

            var gameProviderInfo = new GameProviderInfo(
                GameProviderId: Guid.NewGuid(),
                Status: GameProviderStatus.Active,
                Description: string.Empty,
                AutoCloseRound: true,
                SupportTestPlayer: true,
                AllowKickPlayer: true,
                PlaceBetTimeout: 0,
                SettleBetTimeout: 0,
                UniqueId: 1,
                TransferWallet: true,
                GameProviderCode: gameProviderCode,
                Name: gameProviderCode,
                Localization: null);
            _ = fakeGameGameProviderService.GetAsync(
                Arg.Is(gameProviderCode)).Returns(gameProviderInfo);

            var gameProviderBalance = 0.006m;
            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(playerCode)).Returns(gameProviderBalance);

            var service = new FundInCommandService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakeLogger,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);
        }

        [Test]
        public async Task Game_Provider_Balance_0_Transferred_To_Main_Wallet()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
            var fakeLogger = NullLogger<FundInCommandService>.Instance;
            var fakePlayerService = Substitute.For<IPlayerService>();

            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "KY";
            var defaultWalletId = Guid.NewGuid();

            var playerInfo = new PlayerInfo(
                ResellerId: Guid.NewGuid(),
                ResellerCode: "TGS",
                LicenseeId: Guid.NewGuid(),
                LicenseeCode: "TGS",
                BrandId: Guid.NewGuid(),
                BrandCode: "GPI",
                PlayerCode: playerCode,
                Username: playerCode,
                DefaultWalletId: defaultWalletId,
                DefaultWalletCurrencyCode: "CNY",
                Status: PlayerStatus.Enabled);
            _ = fakePlayerService.GetAsync(
                Arg.Is(playerCode)).Returns(playerInfo);

            var gameProviderInfo = new GameProviderInfo(
                GameProviderId: Guid.NewGuid(),
                Status: GameProviderStatus.Active,
                Description: string.Empty,
                AutoCloseRound: true,
                SupportTestPlayer: true,
                AllowKickPlayer: true,
                PlaceBetTimeout: 0,
                SettleBetTimeout: 0,
                UniqueId: 1,
                TransferWallet: true,
                GameProviderCode: gameProviderCode,
                Name: gameProviderCode,
                Localization: null);
            _ = fakeGameGameProviderService.GetAsync(
                Arg.Is(gameProviderCode)).Returns(gameProviderInfo);

            var gameProviderBalance = 0;
            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(playerCode)).Returns(gameProviderBalance);

            var service = new FundInCommandService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakeLogger,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);
        }

        [Test]
        public async Task Game_Provider_Balance_100_Transferred_To_Main_Wallet()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
            var fakeLogger = NullLogger<FundInCommandService>.Instance;
            var fakePlayerService = Substitute.For<IPlayerService>();

            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "KY";
            var defaultWalletId = Guid.NewGuid();

            var playerInfo = new PlayerInfo(
                ResellerId: Guid.NewGuid(),
                ResellerCode: "TGS",
                LicenseeId: Guid.NewGuid(),
                LicenseeCode: "TGS",
                BrandId: Guid.NewGuid(),
                BrandCode: "GPI",
                PlayerCode: playerCode,
                Username: playerCode,
                DefaultWalletId: defaultWalletId,
                DefaultWalletCurrencyCode: "CNY",
                Status: PlayerStatus.Enabled);
            _ = fakePlayerService.GetAsync(
                Arg.Is(playerCode)).Returns(playerInfo);

            var gameProviderInfo = new GameProviderInfo(
                GameProviderId: Guid.NewGuid(),
                Status: GameProviderStatus.Active,
                Description: string.Empty,
                AutoCloseRound: true,
                SupportTestPlayer: true,
                AllowKickPlayer: true,
                PlaceBetTimeout: 0,
                SettleBetTimeout: 0,
                UniqueId: 1,
                TransferWallet: true,
                GameProviderCode: gameProviderCode,
                Name: gameProviderCode,
                Localization: null);
            _ = fakeGameGameProviderService.GetAsync(
                Arg.Is(gameProviderCode)).Returns(gameProviderInfo);

            var gameProviderBalance = 100m;
            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(playerCode)).Returns(gameProviderBalance);

            var amount = gameProviderBalance;

            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            _ = fakeGameProviderFundTransferTransactionRepository.AddFundInAsync(
                Arg.Is(gameProviderInfo.GameProviderId),
                Arg.Is(defaultWalletId),
                Arg.Is(playerInfo.PlayerCode),
                Arg.Is(gameProviderCode),
                Arg.Is(amount),
                Arg.Any<string>()
                ).Returns(gameProviderFundTransferTransactionId);

            var gameProviderTransactionId = Guid.NewGuid().ToString("N");
            _ = fakeGameProvidersClient.PlayerFundInAsync(
                Arg.Is(gameProviderInfo.GameProviderCode),
                Arg.Is(playerInfo.BrandCode),
                Arg.Is(playerInfo.PlayerCode),
                Arg.Is(playerInfo.DefaultWalletCurrencyCode),
                Arg.Is(gameProviderFundTransferTransactionId.ToString("N")),
                Arg.Is(amount)).Returns(gameProviderTransactionId);

            var transactionResponse = new TransactionResponse()
            {
                TransactionId = Guid.NewGuid(),
                Balance = 100,
                LockedBalance = 0
            };
            _ = fakeCashServiceClient.GameProviderFundInAsync(
                Arg.Is(defaultWalletId),
                Arg.Is(amount),
                Arg.Any<string>(),
                Arg.Is(gameProviderFundTransferTransactionId.ToString())).Returns(transactionResponse);

            var service = new FundInCommandService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakeLogger,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);

            await fakeGameProviderFundTransferTransactionRepository.Received(1)
                .UpdateBySuccessAsync(
                 Arg.Is(gameProviderFundTransferTransactionId),
                 Arg.Is(transactionResponse.TransactionId),
                 Arg.Is(gameProviderTransactionId),
                 Arg.Is(transactionResponse.Balance),
                 Arg.Any<string>()).ConfigureAwait(false);
        }

        [Test]
        public async Task Game_Provider_Balance_123_456_Transferred_To_Main_Wallet_Amount_123_45()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProviderFundTransferTransactionRepository = Substitute.For<IGameProviderFundTransferTransactionRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
            var fakeLogger = NullLogger<FundInCommandService>.Instance;
            var fakePlayerService = Substitute.For<IPlayerService>();

            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "KY";
            var defaultWalletId = Guid.NewGuid();

            var playerInfo = new PlayerInfo(
                ResellerId: Guid.NewGuid(),
                ResellerCode: "TGS",
                LicenseeId: Guid.NewGuid(),
                LicenseeCode: "TGS",
                BrandId: Guid.NewGuid(),
                BrandCode: "GPI",
                PlayerCode: playerCode,
                Username: playerCode,
                DefaultWalletId: defaultWalletId,
                DefaultWalletCurrencyCode: "CNY",
                Status: PlayerStatus.Enabled);
            _ = fakePlayerService.GetAsync(
                Arg.Is(playerCode)).Returns(playerInfo);

            var gameProviderInfo = new GameProviderInfo(
                GameProviderId: Guid.NewGuid(),
                Status: GameProviderStatus.Active,
                Description: string.Empty,
                AutoCloseRound: true,
                SupportTestPlayer: true,
                AllowKickPlayer: true,
                PlaceBetTimeout: 0,
                SettleBetTimeout: 0,
                UniqueId: 1,
                TransferWallet: true,
                GameProviderCode: gameProviderCode,
                Name: gameProviderCode,
                Localization: null);
            _ = fakeGameGameProviderService.GetAsync(
                Arg.Is(gameProviderCode)).Returns(gameProviderInfo);

            var gameProviderBalance = 123.456m;
            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(playerCode)).Returns(gameProviderBalance);

            var amount = 123.45m;

            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            _ = fakeGameProviderFundTransferTransactionRepository.AddFundInAsync(
                Arg.Is(gameProviderInfo.GameProviderId),
                Arg.Is(defaultWalletId),
                Arg.Is(playerInfo.PlayerCode),
                Arg.Is(gameProviderCode),
                Arg.Is(amount),
                Arg.Any<string>()
                ).Returns(gameProviderFundTransferTransactionId);

            var gameProviderTransactionId = Guid.NewGuid().ToString("N");
            _ = fakeGameProvidersClient.PlayerFundInAsync(
                Arg.Is(gameProviderInfo.GameProviderCode),
                Arg.Is(playerInfo.BrandCode),
                Arg.Is(playerInfo.PlayerCode),
                Arg.Is(playerInfo.DefaultWalletCurrencyCode),
                Arg.Is(gameProviderFundTransferTransactionId.ToString("N")),
                Arg.Is(amount)).Returns(gameProviderTransactionId);

            var transactionResponse = new TransactionResponse()
            {
                TransactionId = Guid.NewGuid(),
                Balance = 100,
                LockedBalance = 0
            };
            _ = fakeCashServiceClient.GameProviderFundInAsync(
                Arg.Is(defaultWalletId),
                Arg.Is(amount),
                Arg.Any<string>(),
                Arg.Is(gameProviderFundTransferTransactionId.ToString())).Returns(transactionResponse);

            var service = new FundInCommandService(
                fakeCashServiceClient,
                fakeGameProviderFundTransferTransactionRepository,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakeLogger,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);

            await fakeGameProviderFundTransferTransactionRepository.Received(1)
                .UpdateBySuccessAsync(
                 Arg.Is(gameProviderFundTransferTransactionId),
                 Arg.Is(transactionResponse.TransactionId),
                 Arg.Is(gameProviderTransactionId),
                 Arg.Is(transactionResponse.Balance),
                 Arg.Any<string>()).ConfigureAwait(false);
        }
    }
}
