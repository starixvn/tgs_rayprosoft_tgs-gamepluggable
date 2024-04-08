using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core.Exceptions;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class VerificationRepositoryVerifyPlayerBalanceUnitTest
{
	[Fact]
	public async Task Brand_Currency_Is_Empty()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(string.Empty);

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task Brand_Currency_Is_Null()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((string?)null);

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task Brand_State_Is_Not_Active()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Maintenance);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task GameProvider_Currency_Is_Empty()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(string.Empty);

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task GameProvider_State_Is_Not_Active()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Maintenance);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task Licensee_State_Is_Not_Active()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Maintenance);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		await repository.VerifyPlayerBalanceAsync(
			player: player,
			gameProvider: gameProvider);
	}

	[Fact]
	public async Task Player_State_Is_Disabled()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Disabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		await repository.VerifyPlayerBalanceAsync(
			player: player,
			gameProvider: gameProvider);
	}

	[Fact]
	public async Task Player_State_Is_InBlacklist()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.InBlacklist);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			 .AsTask());
	}

	[Fact]
	public async Task Reseller_State_Is_Not_Active()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Maintenance);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(PlayerStatus.Enabled);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}

	[Fact]
	public async Task State_Is_Null()
	{
		var fakeClinet = Substitute.For<IGameCommandClient>();

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");

		var gameProvider = new GameProviderInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "KY",
			UniqueId: 1,
			Name: "KY",
			TransferWallet: true,
			AutoCloseRound: false,
			SupportTestPlayer: false,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			Description: "KY",
			Localization: string.Empty);

		_ = fakeClinet
			.PlayerGetAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeClinet
			.ResellerGetStatusAsync(
				resellerId: Arg.Is(player.ResellerId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ResellerStatus.Active);

		_ = fakeClinet
			.LicenseeGetStatusAsync(
				licenseeId: Arg.Is(player.LicenseeId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(LicenseeStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.BrandGetStatusAsync(
				brandId: Arg.Is(player.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(BrandStatus.Active);

		_ = fakeClinet
			.PlayerGetStatusAsync(
				playerCode: Arg.Is(player.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerStatus?)null);

		_ = fakeClinet
			.GameProviderGetStatusAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(GameProviderStatus.Active);

		_ = fakeClinet
			.BrandGetCurrencyAsync(
				brandId: Arg.Is(player.BrandId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		_ = fakeClinet
			.GameProviderGetCurrencyCodeAsync(
				gameProviderId: Arg.Is(gameProvider.GameProviderId),
				currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("CNY");

		var repository = new VerificationRepository(
			fakeClinet);

		_ = await Assert.ThrowsAsync<VerifyPlayerBalanceException>(
			() => repository.VerifyPlayerBalanceAsync(
				player: player,
				gameProvider: gameProvider)
			.AsTask());
	}
}
