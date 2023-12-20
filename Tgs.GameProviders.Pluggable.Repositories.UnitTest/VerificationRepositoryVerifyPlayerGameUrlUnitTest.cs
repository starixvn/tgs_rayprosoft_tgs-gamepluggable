using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core.Exceptions;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class VerificationRepositoryVerifyPlayerGameUrlUnitTest
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(string.Empty);

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((string?)null);

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
            .AsTask());
    }

    [Fact]
    public async Task Game_Available_Currency_Is_CNY()
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: ["PHP", "HKD", "CNY"],
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        await repository.VerifyPlayerGameUrlAsync(
            player: player,
            game: game);
    }

    [Fact]
    public async Task Game_Available_Currency_Is_Not_CNY()
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: ["PHP", "HKD"],
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
            .AsTask());
    }

    [Fact]
    public async Task Game_State_Is_Not_Active()
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Maintenance);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(string.Empty);

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Maintenance);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        await repository.VerifyPlayerGameUrlAsync(
            player: player,
            game: game);
    }

    [Fact]
    public async Task Player_State_Is_Not_Enabled()
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameExternalId: "lobby",
            GameType.CardGameLobby,
            GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: "KY",
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: "KY");

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
                gameProviderId: Arg.Is(game.GameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .GameGetStatusAsync(
                gameid: Arg.Is(game.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(player.BrandId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(game.GameProviderId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyPlayerGameUrlException>(
            () => repository.VerifyPlayerGameUrlAsync(
                player: player,
                game: game)
            .AsTask());
    }
}
