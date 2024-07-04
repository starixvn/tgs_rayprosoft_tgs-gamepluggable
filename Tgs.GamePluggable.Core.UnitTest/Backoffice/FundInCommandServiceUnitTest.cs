using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GamePluggable.Core.Exceptions;
using Tgs.GamePluggable.Core.Models;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class FundInCommandServiceUnitTest
{
	[Fact]
	public async Task Verify_Brand_Exception()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		_ = fakeVerification
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException(new VerifyBrandException(playerInfo.BrandCode, gameProviderInfo.GameProviderCode, playerInfo.CurrencyCode)));

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await Assert.ThrowsAsync<VerifyBrandException>(() =>
		sut.ExecuteAsync(
			command)
		.AsTask());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundBalanceAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferInAsync(
				gameProviderCode: Arg.Any<string>(),
				playerCode: Arg.Any<string>(),
				walletId: Arg.Any<Guid>(),
				currencyCode: Arg.Any<string>(),
				amount: Arg.Any<decimal>(),
				cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_Balance_0_006_Transferred_To_Main_Wallet()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1000.0m,
			FreeBalance = 0.006m,
			OutstandingBalance = 999.994m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferInAsync(
				gameProviderCode: Arg.Any<string>(),
				playerCode: Arg.Any<string>(),
				walletId: Arg.Any<Guid>(),
				currencyCode: Arg.Any<string>(),
				amount: Arg.Any<decimal>(),
				cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_Balance_0_Transferred_To_Main_Wallet()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1000.00m,
			FreeBalance = 0.00m,
			OutstandingBalance = 1000.00m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferInAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			walletId: Arg.Any<Guid>(),
			currencyCode: Arg.Any<string>(),
			amount: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_Balance_100_Transferred_To_Main_Wallet()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1000.00m,
			FreeBalance = 100.00m,
			OutstandingBalance = 900.00m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundTransferInAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			walletId: Arg.Is(playerInfo.WalletId),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			amount: Arg.Is(100.00m),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_Balance_123_456_Transferred_To_Main_Wallet_Amount_123_45()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1000.00m,
			FreeBalance = 123.456m,
			OutstandingBalance = 876.554m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundTransferInAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			walletId: Arg.Is(playerInfo.WalletId),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			amount: Arg.Is(123.45m),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_SM01_Balance_0_006_Can_Not_Transferred_To_Main_Wallet()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "SM01");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1000.00m,
			FreeBalance = 0.006m,
			OutstandingBalance = 999.994m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferInAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			walletId: Arg.Any<Guid>(),
			currencyCode: Arg.Any<string>(),
			amount: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_SM01_Balance_0_Can_Not_Transferred_To_Main_Wallet()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "SM01");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 0.00m,
			FreeBalance = 0.00m,
			OutstandingBalance = 0.00m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundTransferInAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			walletId: Arg.Any<Guid>(),
			currencyCode: Arg.Any<string>(),
			amount: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_SM01_Balance_123_456_Transferred_To_Main_Wallet_Amount_123()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "SM01");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 1000.00m,
			FreeBalance = 123.456m,
			OutstandingBalance = 876.544m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundTransferInAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			walletId: Arg.Is(playerInfo.WalletId),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			amount: Arg.Is(123M),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Game_Provider_Balance_10000001_Transferred_To_Main_Wallet_Amount_10000000()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var command = new FundInCommand(
			 PlayerCode: "4ucmqplxb029",
			 GameProviderCode: "KY");

		var playerInfo = new PlayerInfo(
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(command.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		var fundBalanceReply = new GameFundTransfer.Protos.FundBalanceReply
		{
			Balance = 10000001.00m,
			FreeBalance = 10000001.00m,
			OutstandingBalance = 0.00m
		};
		_ = fakeGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new FundInCommandService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await sut.ExecuteAsync(command);

		_ = fakeVerification
			.Received(1)
			.VerifyBrandAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameFundTransferClient
			.Received(1)
			.FundTransferInAsync(
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			walletId: Arg.Is(playerInfo.WalletId),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			amount: Arg.Is(10000000.00m),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
