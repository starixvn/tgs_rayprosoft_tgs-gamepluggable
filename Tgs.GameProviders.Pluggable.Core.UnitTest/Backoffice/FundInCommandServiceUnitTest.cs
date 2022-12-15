using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Backoffice;

[TestFixture]
public class FundInCommandServiceUnitTest
{
    [Test]
    public async Task Game_Provider_Balance_0_006_Transferred_To_Main_Wallet()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "KY");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 0.006m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeGameFundTransferClient
            .Received(0)
            .FundInAsync(
                Arg.Any<GameFundTransferFundRequest>());
    }

    [Test]
    public async Task Game_Provider_Balance_0_Transferred_To_Main_Wallet()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "KY");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 0m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeGameFundTransferClient
            .Received(0)
            .FundInAsync(
                Arg.Any<GameFundTransferFundRequest>());
    }

    [Test]
    public async Task Game_Provider_Balance_100_Transferred_To_Main_Wallet()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "KY");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 100m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundInAsync(
                Arg.Any<GameFundTransferFundRequest>());
    }

    [Test]
    public async Task Game_Provider_Balance_123_456_Transferred_To_Main_Wallet_Amount_123_45()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "KY");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 123.456m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        var request = new GameFundTransferFundRequest()
        {
            GameProviderId = gameProvider.GameProviderId,
            GameProviderCode = gameProvider.GameProviderCode,
            BrandCode = player.BrandCode,
            PlayerCode = player.PlayerCode,
            CurrencyCode = player.DefaultWalletCurrencyCode,
            WalletId = player.DefaultWalletId,
            Amount = 123.45m
        };

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundInAsync(
                Arg.Is(request));
    }

    [Test]
    public async Task Game_Provider_SM01_Balance_0_006_Can_Not_Transferred_To_Main_Wallet()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "SM01");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 0.006m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeGameFundTransferClient
            .Received(0)
            .FundInAsync(
                Arg.Any<GameFundTransferFundRequest>());
    }

    [Test]
    public async Task Game_Provider_SM01_Balance_0_Can_Not_Transferred_To_Main_Wallet()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "SM01");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 0m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeGameFundTransferClient
            .Received(0)
            .FundInAsync(
                Arg.Any<GameFundTransferFundRequest>());
    }

    [Test]
    public async Task Game_Provider_SM01_Balance_123_456_Transferred_To_Main_Wallet_Amount_123()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundInCommand(
             PlayerCode: "4ucmqplxb029",
             GameProviderCode: "SM01");

        var player = new PlayerInfo(
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: command.PlayerCode,
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
            .Returns(gameProviderId);

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: command.GameProviderCode);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(gameProviderId))
            .Returns(gameProvider);

        var gameProviderBalance = 123.456m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(gameProvider.GameProviderCode),
            Arg.Is(player.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var service = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakePlayer,
            fakePlayerGameBalance);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        var request = new GameFundTransferFundRequest()
        {
            GameProviderId = gameProvider.GameProviderId,
            GameProviderCode = gameProvider.GameProviderCode,
            BrandCode = player.BrandCode,
            PlayerCode = player.PlayerCode,
            CurrencyCode = player.DefaultWalletCurrencyCode,
            WalletId = player.DefaultWalletId,
            Amount = 123M
        };

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundInAsync(
                Arg.Is(request));
        
    }
}
