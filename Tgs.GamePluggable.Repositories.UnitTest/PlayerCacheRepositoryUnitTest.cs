using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Models;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class PlayerCacheRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

		var playerCode = "4ucmqplxb029";

		var key = string.Concat(typeof(PlayerCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "player", ":", "get", ":", playerCode);

		_ = fakeMemoryCache
			.TryGetValue(
				key: Arg.Is(key),
				value: out _)
			 .Returns(false);

		var player = new PlayerInfo(
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
		_ = fakePlayer
			.GetAsync(
				playerCode: Arg.Is(playerCode))
			.Returns(player);

		var repository = new PlayerCacheRepository(
			fakePlayer,
			fakeMemoryCache,
			absoluteExpirationRelativeToNow);

		_ = await repository.GetAsync(playerCode);

		_ = fakeMemoryCache
			.Received(1)
			.Set(
				key: key,
				player);
	}
}
