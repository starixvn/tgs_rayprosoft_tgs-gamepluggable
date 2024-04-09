using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class PlayerRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository.Get")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
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
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
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

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 0,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddAsync(reseller);
		await context.Licensees.AddAsync(licensee);
		await context.Brands.AddAsync(brand);
		await context.Players.AddAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		var actual = await repository.GetAsync("4ucmqplxb029");

		Assert.NotNull(actual);
		Assert.Equal(brand.Id, actual.BrandId);
		Assert.Equal(brand.Code, actual.BrandCode);
		Assert.Equal(player.PlayerCode, actual.PlayerCode);
		Assert.Equal(player.Username, actual.Username);
		Assert.Equal(player.DefaultWalletId, actual.WalletId);
		Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
	}

	[Fact]
	public async Task Get_ArgumentNullException()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository.Get_ArgumentNullException")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
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
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 0,
			CreatedOn = DateTime.Now,
			CreatedByUsername = string.Empty,
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = string.Empty,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N")
		};

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 0,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddRangeAsync(reseller);
		await context.Licensees.AddRangeAsync(licensee);
		await context.Brands.AddRangeAsync(brand);
		await context.Players.AddRangeAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(
			() => repository.GetAsync("4ucmqplxe65x").AsTask());
	}

	[Fact]
	public async Task GetStatus_Is_True()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository..GetStatus_Is_True")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
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
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
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

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 0,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddRangeAsync(reseller);
		await context.Licensees.AddRangeAsync(licensee);
		await context.Brands.AddRangeAsync(brand);
		await context.Players.AddRangeAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		var actual = await repository.GetStatusAsync("4ucmqplxb029");

		Assert.True(actual);
	}

	[Fact]
	public async Task GetStatus_Is_False_Reseller_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository.GetStatus_Is_False_Reseller_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
		var reseller = new Reseller()
		{
			Id = resellerId,
			Code = resellerCode,
			Name = resellerCode,
			Status = 1,
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
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
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

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 0,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddRangeAsync(reseller);
		await context.Licensees.AddRangeAsync(licensee);
		await context.Brands.AddRangeAsync(brand);
		await context.Players.AddRangeAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		var actual = await repository.GetStatusAsync("4ucmqplxb029");

		Assert.False(actual);
	}

	[Fact]
	public async Task GetStatus_Is_False_Licensee_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository.GetStatus_Is_False_Licensee_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
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
			Status = 1,
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
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

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 0,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddRangeAsync(reseller);
		await context.Licensees.AddRangeAsync(licensee);
		await context.Brands.AddRangeAsync(brand);
		await context.Players.AddRangeAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		var actual = await repository.GetStatusAsync("4ucmqplxb029");

		Assert.False(actual);
	}

	[Fact]
	public async Task GetStatus_Is_False_Brand_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository..GetStatus_Is_False_Brand_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
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
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var brand = new Brand()
		{
			Id = brandId,
			Code = brandCode,
			Name = brandCode,
			LicenseeId = licenseeId,
			Status = 1,
			ApiKey = Guid.NewGuid().ToString("N"),
			SecretKey = Guid.NewGuid().ToString("N"),
			ClientId = Guid.NewGuid().ToString("N"),
			CreatedOn = DateTime.Now,
			CreatedByUsername = "admin.enabled",
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 0,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddRangeAsync(reseller);
		await context.Licensees.AddRangeAsync(licensee);
		await context.Brands.AddRangeAsync(brand);
		await context.Players.AddRangeAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		var actual = await repository.GetStatusAsync("4ucmqplxb029");

		Assert.False(actual);
	}

	[Fact]
	public async Task GetStatus_Is_False_Player_Is_Not_0()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
			.UseInMemoryDatabase(databaseName: "dbo.PlayerRepository..GetStatus_Is_False_Player_Is_Not_0")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var fakeRepository = new PlayerRepository(context);

		var resellerId = Guid.NewGuid();
		var resellerCode = "TGS";
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
		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
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

		var player = new Player()
		{
			PlayerCode = "4ucmqplxb029",
			Username = "4ucmqplxb029",
			BrandId = brandId,
			Status = 1,
			RealName = "4ucmqplxb029",
			Password = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawPassword = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			WithdrawSalt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			Salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
			DefaultWalletCurrencyCode = "CNY",
			DefaultWalletId = Guid.NewGuid(),
			UpdatedOn = DateTime.Now,
			UpdatedByUsername = "admin.enabled"
		};
		await context.Resellers.AddRangeAsync(reseller);
		await context.Licensees.AddRangeAsync(licensee);
		await context.Brands.AddRangeAsync(brand);
		await context.Players.AddRangeAsync(player);
		_ = await context.SaveChangesAsync();

		var repository = new PlayerRepository(context);

		var actual = await repository.GetStatusAsync("4ucmqplxb029");

		Assert.False(actual);
	}
}
