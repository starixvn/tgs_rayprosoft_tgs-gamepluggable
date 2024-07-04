using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GamePluggable.Core.Exceptions;
using Tgs.GamePluggable.Core.Models;

namespace Tgs.GamePluggable.Core.UnitTest.Backoffice;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var query = new PlayerBalanceQuery(
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
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(query.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: query.GameProviderCode,
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

		var sut = new PlayerBalanceGetService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		var actual = await sut.GetAsync(query);

		Assert.Equal(fundBalanceReply.Balance.ToDecimal().GetValueOrDefault(0.0m), actual.Balance);
		Assert.Equal(fundBalanceReply.FreeBalance.ToDecimal().GetValueOrDefault(0.0m), actual.FreeBalance);
		Assert.Equal(fundBalanceReply.OutstandingBalance.ToDecimal().GetValueOrDefault(0.0m), actual.OutstandingBalance);

		_ = fakeVerification
			.Received(1)
			.VerifyBackofficePlayerBalanceAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Verify_Backoffice_Player_Balance_Exception()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakeVerification = Substitute.For<IVerification>();

		var query = new PlayerBalanceQuery(
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
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(query.GameProviderCode),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var gameProviderInfo = new GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: query.GameProviderCode,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProviderRepository.GetAsync(
			Arg.Is(gameProviderId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderInfo);

		_ = fakeVerification
			.VerifyBackofficePlayerBalanceAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			brandCode: Arg.Is(playerInfo.BrandCode),
			gameProviderId: Arg.Is(gameProviderInfo.GameProviderId),
			gameProviderCode: Arg.Is(gameProviderInfo.GameProviderCode),
			transferWallet: Arg.Is(gameProviderInfo.TransferWallet),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException(new VerifyBackofficePlayerBalanceException(playerInfo.BrandCode, gameProviderInfo.GameProviderCode, playerInfo.CurrencyCode)));

		var sut = new PlayerBalanceGetService(
			fakePlayerRepository,
			fakeGameProviderRepository,
			fakeVerification,
			fakeGameFundTransferClient);

		await Assert.ThrowsAsync<VerifyBackofficePlayerBalanceException>(() =>
		sut.GetAsync(
			query)
		.AsTask());

		_ = fakeGameFundTransferClient
			.Received(0)
			.FundBalanceAsync(
			gameProviderCode: Arg.Any<string>(),
			playerCode: Arg.Any<string>(),
			Arg.Any<CancellationToken>());
	}
}
