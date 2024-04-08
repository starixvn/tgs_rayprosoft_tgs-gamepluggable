using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class GameProviderRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameProviderRepository.Get")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var gameProviderId = Guid.NewGuid();
		var gameProvider = new GameProvider
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
			Code = "KY",
			Name = "KY",
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Localization = null
		};

		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new GameProviderRepository(context);
		var actual = await sut.GetAsync(gameProviderId);

		Assert.NotNull(actual);
		Assert.Equal(gameProvider.Id, actual.GameProviderId);
		Assert.Equal(gameProvider.Code, actual.GameProviderCode);
		Assert.Equal(gameProvider.TransferWallet, actual.TransferWallet);
		Assert.Equal(gameProvider.AllowKickPlayer, actual.AllowKickPlayer);
	}

	[Fact]
	public async Task Get_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameProviderRepository.Get_ArgumentNullException")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var gameProviderId = Guid.NewGuid();
		var gameProvider = new GameProvider
		{
			Id = Guid.NewGuid(),
			Status = 0,
			Description = string.Empty,
			AutoCloseRound = false,
			SupportTestPlayer = false,
			AllowKickPlayer = true,
			PlaceBetTimeout = 0,
			SettleBetTimeout = 0,
			UniqueId = 1,
			TransferWallet = true,
			Code = "KY",
			Name = "KY",
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Localization = null
		};

		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new GameProviderRepository(context);
		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetAsync(gameProviderId).AsTask());
	}

	[Fact]
	public async Task GetConfiguration()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameProviderRepository.GetConfiguration")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "BC";
		var gameProvider = new GameProvider()
		{
			Id = gameProviderId,
			Status = 0,
			Description = "BetConstruct",
			AutoCloseRound = true,
			SupportTestPlayer = true,
			AllowKickPlayer = false,
			PlaceBetTimeout = 0,
			SettleBetTimeout = 0,
			UniqueId = 1,
			TransferWallet = true,
			Code = gameProviderCode,
			Name = "BetConstruct",
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = "admin.enabled",
			UpdatedByUsername = "admin.enabled",
			Localization = null,
		};
		var gameProviderConfigId = Guid.NewGuid();
		var gameProviderConfig = new GameProviderConfig
		{
			Id = gameProviderConfigId,
			Name = "Configuration BetConstruct",
			Description = "BetConstruct",
			ClientId = "BetConstruct-PG",
			SecretKey = "jiLEPYw5n3n5kcx6T3",
			Status = 0,
			GameProviderId = gameProviderId,
			DesktopEndpoint = "https://sportsbook.srvttt.com",
			MobileEndpoint = "https://mobile.srvttt.com",
			ConfigurationData = string.Empty,
			IsDefault = true,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = "admin.enabled",
			UpdatedByUsername = "admin.enabled"
		};
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();
		await context.GameProviderConfigs.AddAsync(gameProviderConfig);
		_ = await context.SaveChangesAsync();

		var sut = new GameProviderRepository(context);
		var actual = await sut.GetConfigurationAsync(gameProviderConfigId);

		Assert.NotNull(actual);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(gameProviderConfig.Id, actual.GameProviderConfigId);
		Assert.Equal(gameProviderConfig.Status, (int)actual.Status);
		Assert.Equal(gameProviderConfig.Name, actual.Name);
		Assert.Equal(gameProviderConfig.Description, actual.Description);
		Assert.Equal(gameProviderConfig.ClientId, actual.ClientId);
		Assert.Equal(gameProviderConfig.SecretKey, actual.SecretKey);
		Assert.Equal(gameProviderConfig.DesktopEndpoint, actual.DesktopEndpoint);
		Assert.Equal(gameProviderConfig.MobileEndpoint, actual.MobileEndpoint);
		Assert.Equal(gameProviderConfig.ConfigurationData, actual.ConfigurationData);
		Assert.Equal(gameProviderConfig.IsDefault, actual.IsDefault);
	}

	[Fact]
	public async Task GetConfiguration_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameProviderRepository.GetConfiguration_ArgumentNullException")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "BC";
		var gameProvider = new GameProvider()
		{
			Id = gameProviderId,
			Status = 0,
			Description = "BetConstruct",
			AutoCloseRound = true,
			SupportTestPlayer = true,
			AllowKickPlayer = false,
			PlaceBetTimeout = 0,
			SettleBetTimeout = 0,
			UniqueId = 1,
			TransferWallet = true,
			Code = gameProviderCode,
			Name = "BetConstruct",
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = "admin.enabled",
			UpdatedByUsername = "admin.enabled",
			Localization = null,
		};
		var gameProviderConfigId = Guid.NewGuid();
		var gameProviderConfig = new GameProviderConfig
		{
			Id = Guid.NewGuid(),
			Name = "Configuration BetConstruct",
			Description = "BetConstruct",
			ClientId = "BetConstruct-PG",
			SecretKey = "jiLEPYw5n3n5kcx6T3",
			Status = 0,
			GameProviderId = gameProviderId,
			DesktopEndpoint = "https://sportsbook.srvttt.com",
			MobileEndpoint = "https://mobile.srvttt.com",
			ConfigurationData = string.Empty,
			IsDefault = true,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = "admin.enabled",
			UpdatedByUsername = "admin.enabled"
		};
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();
		await context.GameProviderConfigs.AddAsync(gameProviderConfig);
		_ = await context.SaveChangesAsync();

		var sut = new GameProviderRepository(context);
		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetConfigurationAsync(gameProviderConfigId).AsTask());
	}

	[Fact]
	public async Task GetGameProviderId()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameProviderRepository.GetGameProviderId")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var gameProvider = new GameProvider
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
			Code = "KY",
			Name = "KY",
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Localization = null
		};

		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new GameProviderRepository(context);
		var actual = await sut.GetGameProviderIdAsync(gameProviderCode);

		Assert.Equal(gameProvider.Id, gameProviderId);
	}

	[Fact]
	public async Task GetGameProviderId_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.GameProviderRepository.GetGameProviderId_ArgumentNullException")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "VR";
		var gameProvider = new GameProvider
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
			Code = "KY",
			Name = "KY",
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			CreatedByUsername = string.Empty,
			UpdatedByUsername = string.Empty,
			Localization = null
		};

		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new GameProviderRepository(context);
		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetGameProviderIdAsync(gameProviderCode).AsTask());
	}
}
