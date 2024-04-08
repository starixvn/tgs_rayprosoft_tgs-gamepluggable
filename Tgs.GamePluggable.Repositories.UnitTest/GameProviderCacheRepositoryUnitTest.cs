using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GamePluggable.Core;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class GameProviderCacheRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();

		var gameProviderId = Guid.NewGuid();

		var key = string.Concat(typeof(GameProviderCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "gameprovider", ":", "get", ":", gameProviderId);

		_ = fakeMemoryCache
			.TryGetValue(
				key: Arg.Is(key),
				value: out _)
			 .Returns(false);

		var gameProvider = new Core.Models.GameProviderInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.Kaiyuan,
			TransferWallet: true,
			AllowKickPlayer: true);
		_ = fakeGameProvider
			.GetAsync(
				id: Arg.Is(gameProviderId))
			.Returns(gameProvider);

		var repository = new GameProviderCacheRepository(
			fakeGameProvider,
			fakeMemoryCache);

		var actual = await repository.GetAsync(gameProviderId);

		Assert.NotNull(actual);

		_ = fakeMemoryCache
			.Received(1)
			.Set(
				key: key,
				gameProvider);
	}

	[Fact]
	public async Task GetConfiguration()
	{
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();

		var gameProviderConfigId = Guid.NewGuid();

		var gameProviderConfiguration = new Core.Models.GameProviderConfigInfo(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.BetConstruct,
			GameProviderConfigId: gameProviderConfigId,
			Status: Core.Enums.GameProviderConfigStatus.Active,
			Name: $"{GameProviderType.BetConstruct} Configuration",
			Description: $"{GameProviderType.BetConstruct} Configuration",
			ClientId: $"{GameProviderType.BetConstruct}-GPI",
			SecretKey: Guid.NewGuid().ToString("N"),
			DesktopEndpoint: "https://devsportsbook.srvttt.com",
			MobileEndpoint: "https://devmobile.srvttt.com",
			ConfigurationData: string.Empty,
			IsDefault: false);
		_ = fakeGameProvider
			.GetConfigurationAsync(
				gameProviderConfigId: Arg.Is(gameProviderConfigId))
			.Returns(gameProviderConfiguration);

		var repository = new GameProviderCacheRepository(
			fakeGameProvider,
			fakeMemoryCache);

		var actual = await repository.GetConfigurationAsync(gameProviderConfigId);

		Assert.NotNull(actual);
	}

	[Fact]
	public async Task GetGameProviderId()
	{
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeMemoryCache = Substitute.For<IMemoryCache>();
		var gameProviderCode = "KY";

		var key = string.Concat(typeof(GameProviderCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "gameprovider", ":", "get", ":", "id", ":", gameProviderCode);

		_ = fakeMemoryCache
			.TryGetValue(
				key: Arg.Is(key),
				value: out _)
			 .Returns(false);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
				code: Arg.Is(gameProviderCode))
			.Returns(gameProviderId);

		var repository = new GameProviderCacheRepository(
			fakeGameProvider,
			fakeMemoryCache);

		var actual = await repository.GetGameProviderIdAsync(gameProviderCode);

		Assert.Equal(gameProviderId, actual);

		_ = fakeMemoryCache
			.Received(1)
			.Set(
				key: key,
				gameProviderId);
	}
}
