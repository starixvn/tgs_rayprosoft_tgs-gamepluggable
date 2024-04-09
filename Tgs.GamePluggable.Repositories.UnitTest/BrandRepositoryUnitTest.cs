using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Tgs.GamePluggable.Core.Enums;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class BrandRepositoryUnitTest
{
	[Fact]
	public async Task GetGameProviderConfigurationId()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderConfigurationId")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new BrandRepository(context);
		var gameProviderId = Guid.NewGuid();
		var brandId = Guid.NewGuid();
		var gameProviderConfigurationId = Guid.NewGuid();
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			GameProviderId = gameProviderId,
			BrandId = brandId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = string.Empty,
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};

		await context.BrandGameProviders.AddAsync(brandGameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderConfigurationIdAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.Equal(gameProviderConfigurationId, actual);
	}

	[Fact]
	public async Task GetCurrencyExist_Is_True()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetCurrencyExist_Is_True")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var brandId = Guid.NewGuid();
		var currencyCode = "CNY";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brandCurrency = new BrandCurrency()
		{
			BrandId = brandId,
			CurrencyCode = "CNY",
			BrandCurrencyCode = "CNY"
		};

		await context.BrandCurrencies.AddAsync(brandCurrency);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetCurrencyExistAsync(
			brandId: brandId,
			currencyCode: currencyCode);

		Assert.True(actual);
	}

	[Fact]
	public async Task GetCurrencyExist_Is_False()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetCurrencyExist_Is_False")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var brandId = Guid.NewGuid();
		var currencyCode = "PHP";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brandCurrency = new BrandCurrency()
		{
			BrandId = brandId,
			CurrencyCode = "CNY",
			BrandCurrencyCode = "CNY"
		};

		await context.BrandCurrencies.AddAsync(brandCurrency);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetCurrencyExistAsync(
			brandId: brandId,
			currencyCode: currencyCode);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_True()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_True")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.True(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_False_BrandGameProvider_Status_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_False_BrandGameProvider_Status_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 1,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_False_GameProvider_Status_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_False_GameProvider_Status_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var gameProvider = new GameProvider()
		{
			Id = gameProviderId,
			Status = 1,
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
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_False_BrandGameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_False_BrandGameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_False_GameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_False_GameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 1,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_False_ResellerGameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_False_ResellerGameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.Resellers.AddAsync(reseller);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameProviderStatus_Is_False_LicenseeGameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameProviderStatus_Is_False_LicenseeGameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.Brands.AddAsync(brand);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameProviderStatusAsync(
			brandId: brandId,
			gameProviderId: gameProviderId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_True()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_True")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.True(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_BrandGame_Status_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_BrandGame_Status_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 1,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_Game_Status_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_BrandGame_Status_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 1,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_GameProvider_Status_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_BrandGame_Status_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

		var gameProvider = new GameProvider()
		{
			Id = gameProviderId,
			Status = 1,
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
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_BrandGameProvider_Status_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_BrandGame_Status_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 1,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_BrandGame_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_BrandGame_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_Game_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_Game_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_BrandGameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_BrandGameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_GameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_GameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_ResellerGameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_ResellerGameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var licenseeGameProvider = new LicenseeGameProvider()
		{
			LicenseeId = licenseeId,
			GameProviderId = gameProviderId
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.Licensees.AddAsync(licensee);
		await context.LicenseeGameProviders.AddAsync(licenseeGameProvider);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}

	[Fact]
	public async Task GetGameStatus_Is_False_LicenseeGameProvider_does_not_exist()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.BrandRepository.GetGameStatus_Is_False_LicenseeGameProvider_does_not_exist")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var gameProviderId = Guid.NewGuid();
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var resellerGameProvider = new ResellerGameProvider
		{
			ResellerId = resellerId,
			GameProviderId = gameProviderId
		};

		var licenseeId = Guid.NewGuid();
		var licenseeCode = "TGS";
		var licensee = new Licensee()
		{
			Id = licenseeId,
			Code = licenseeCode,
			Name = licenseeCode,
			ResellerId = resellerId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderConfigurationId = Guid.NewGuid();
		var gameId = 1;
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandGameProvider = new BrandGameProvider()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			Status = 0,
			DefaultLanguageCode = "en",
			DesktopQueryString = string.Empty,
			MobileQueryString = string.Empty,
			IsManualChangeStatus = false
		};
		var brandGame = new BrandGame()
		{
			Id = Guid.NewGuid(),
			BrandId = brandId,
			GameId = gameId,
			Status = 0,
			Tag = 0,
			DisplayOrder = 99,
			Rebate = true,
			EffectiveBettingPercentage = 1.0m
		};

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
			Code = "KY",
			Name = "KY",
			Localization = null,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var game = new Game
		{
			Id = 1,
			Status = 0,
			ExternalId = "baccarat",
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
			Code = "baccarat",
			Name = "Baccarat",
			Localization = null,
			SubGameProviderCode = "KY"
		};

		await context.Resellers.AddAsync(reseller);
		await context.ResellerGameProviders.AddAsync(resellerGameProvider);
		await context.Licensees.AddAsync(licensee);
		await context.Brands.AddAsync(brand);
		await context.BrandGames.AddAsync(brandGame);
		await context.Games.AddAsync(game);
		await context.BrandGameProviders.AddAsync(brandGameProvider);
		await context.GameProviders.AddAsync(gameProvider);
		_ = await context.SaveChangesAsync();

		var sut = new BrandRepository(context);

		var actual = await sut.GetGameStatusAsync(
			brandId: brandId,
			gameId: gameId);

		Assert.False(actual);
	}
}
