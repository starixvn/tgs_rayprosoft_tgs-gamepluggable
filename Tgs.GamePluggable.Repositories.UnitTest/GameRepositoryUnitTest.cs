using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Tgs.GamePluggable.Core.Enums;
using Tgs.GamePluggable.Entities;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class GameRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameRepository.Get")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new GameRepository(context);

		var gameId = 1;
		var externalGameId = "ky";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var gameProvider = new GameProvider()
		{
			Id = gameProviderId,
			Status = 0,
			Description = string.Empty,
			AutoCloseRound = false,
			SupportTestPlayer = false,
			AllowKickPlayer = true,
			PlaceBetTimeout = 0,
			SettleBetTimeout = 0,
			UniqueId = 1,
			TransferWallet = true,
			Code = gameProviderCode,
			Name = gameProviderCode,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Localization = null,
		};
		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = externalGameId,
			Description = string.Empty,
			EndpointPath = string.Empty,
			RuleUrl = string.Empty,
			GameProviderId = gameProviderId,
			Type = (short)GameType.CardGame,
			PlatformType = (short)GamePlatformType.DesktopAndMobile,
			SubGame = true,
			AvailableToBrand = false,
			HasTurnoverOnTieBet = false,
			Html5 = false,
			AvailableCurrencies = string.Empty,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Code = "ky",
			Name = "ky",
			Localization = null,
			SubGameProviderCode = gameProviderCode
		};

		await context.GameProviders.AddAsync(gameProvider);
		await context.Games.AddAsync(game);
		_ = await context.SaveChangesAsync();

		var sut = new GameRepository(context);

		var actual = await sut.GetAsync(gameId);

		Assert.NotNull(actual);
		Assert.Equal(gameId, actual.GameId);
		Assert.Equal(game.Code, actual.GameCode);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal((int)game.Type, (int)actual.Type);
		Assert.Equal((int)game.PlatformType, (int)actual.PlatformType);
	}

	[Fact]
	public async Task Get_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameRepository.Get_ArgumentNullException")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new GameRepository(context);

		var gameExternalId = "ky";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var gameProvider = new GameProvider()
		{
			Id = gameProviderId,
			Status = 0,
			Description = string.Empty,
			AutoCloseRound = false,
			SupportTestPlayer = false,
			AllowKickPlayer = true,
			PlaceBetTimeout = 0,
			SettleBetTimeout = 0,
			UniqueId = 1,
			TransferWallet = true,
			Code = gameProviderCode,
			Name = gameProviderCode,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Localization = null,
		};
		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = gameExternalId,
			Description = string.Empty,
			EndpointPath = string.Empty,
			RuleUrl = string.Empty,
			GameProviderId = gameProviderId,
			Type = (short)GameType.CardGame,
			PlatformType = (short)GamePlatformType.DesktopAndMobile,
			SubGame = true,
			AvailableToBrand = false,
			HasTurnoverOnTieBet = false,
			Html5 = false,
			AvailableCurrencies = string.Empty,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Code = "ky",
			Name = "ky",
			Localization = null,
			SubGameProviderCode = gameProviderCode
		};

		await context.GameProviders.AddAsync(gameProvider);
		await context.Games.AddAsync(game);
		_ = await context.SaveChangesAsync();

		var sut = new GameRepository(context);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetAsync(2).AsTask());
	}
}
