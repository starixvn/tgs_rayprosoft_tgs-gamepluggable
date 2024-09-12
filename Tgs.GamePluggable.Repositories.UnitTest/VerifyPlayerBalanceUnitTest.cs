using NSubstitute;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Exceptions;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class VerifyPlayerBalanceUnitTest
{
	[Fact]
	public async Task Brand_Currency_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameProviderStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameProviderId: Arg.Is(gameProvider.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(gameProvider.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => sut.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task Player_State_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		_ = fakeBrandRepository.GetGameProviderStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameProviderId: Arg.Is(gameProvider.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(gameProvider.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => sut.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task GameProvider_Currency_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameProviderStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameProviderId: Arg.Is(gameProvider.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(gameProvider.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => sut.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task GameProvider_TransferWallet_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: false,
			AllowKickPlayer: true);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameProviderStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameProviderId: Arg.Is(gameProvider.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(gameProvider.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => sut.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var player = new Core.Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			TransferWallet: true,
			AllowKickPlayer: true);

		_ = fakePlayerRepository.GetStatusAsync(
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetGameProviderStatusAsync(
			brandId: Arg.Is(player.BrandId),
			gameProviderId: Arg.Is(gameProvider.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(player.BrandId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository.GetCurrencyExistAsync(
			id: Arg.Is(gameProvider.GameProviderId),
			currencyCode: Arg.Is(player.CurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		await sut.VerifyPlayerBalanceAsync(
			player: player,
			gameProvider: gameProvider);
	}
}
