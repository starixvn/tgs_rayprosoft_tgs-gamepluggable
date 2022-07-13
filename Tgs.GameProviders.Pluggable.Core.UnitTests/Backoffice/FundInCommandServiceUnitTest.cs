using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
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
            var fakeGameFundTransferGrpcService = Substitute.For<IGameFundTransferGrpcService>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
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
                fakeGameFundTransferGrpcService,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);
        }

        [Test]
        public async Task Game_Provider_Balance_0_Transferred_To_Main_Wallet()
        {
            var fakeGameFundTransferGrpcService = Substitute.For<IGameFundTransferGrpcService>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
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
                fakeGameFundTransferGrpcService,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);
        }

        [Test]
        public async Task Game_Provider_Balance_100_Transferred_To_Main_Wallet()
        {
            var fakeGameFundTransferGrpcService = Substitute.For<IGameFundTransferGrpcService>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
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

            var service = new FundInCommandService(
                fakeGameFundTransferGrpcService,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);

            var request = new GameFundTransferFundRequest()
            {
                GameProviderId = gameProviderInfo.GameProviderId,
                GameProviderCode = gameProviderInfo.GameProviderCode,
                BrandCode = playerInfo.BrandCode,
                PlayerCode = playerInfo.PlayerCode,
                CurrencyCode = playerInfo.DefaultWalletCurrencyCode,
                WalletId = playerInfo.DefaultWalletId,
                Amount = amount
            };

            _ = fakeGameFundTransferGrpcService.Received(1).FundInAsync(request);
        }

        [Test]
        public async Task Game_Provider_Balance_123_456_Transferred_To_Main_Wallet_Amount_123_45()
        {
            var fakeGameFundTransferGrpcService = Substitute.For<IGameFundTransferGrpcService>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameGameProviderService = Substitute.For<IGameProviderService>();
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

            var service = new FundInCommandService(
                fakeGameFundTransferGrpcService,
                fakeGameProvidersClient,
                fakeGameGameProviderService,
                fakePlayerService);

            await service.ExecuteAsync(new FundInCommand(
                 PlayerCode: playerCode,
                 GameProviderCode: gameProviderCode)).ConfigureAwait(false);

            var request = new GameFundTransferFundRequest()
            {
                GameProviderId = gameProviderInfo.GameProviderId,
                GameProviderCode = gameProviderInfo.GameProviderCode,
                BrandCode = playerInfo.BrandCode,
                PlayerCode = playerInfo.PlayerCode,
                CurrencyCode = playerInfo.DefaultWalletCurrencyCode,
                WalletId = playerInfo.DefaultWalletId,
                Amount = amount
            };

            _ = fakeGameFundTransferGrpcService.Received(1).FundInAsync(request);
        }
    }
}
