using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GamePluggable.Core;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class GameUrlRepositoryUnitTest
{
	[Fact]
	public async Task Normal_Process_Game_Url()
	{
		var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

		var gameProviderCode = "KY";
		var playerCode = "Test";
		var gameId = 1;
		var loginRecordId = Guid.NewGuid();
		var language = "zh-TW";
		var platformType = PlatformType.Desktop;
		var ipAddress = "127.0.01";
		var backUrl = new Uri("https://dev-t1.rpgp.cc");

		var gameUrl = new Uri("https://www.google.com/");
		_ = fakeGameProvidersClient.GameUrlAsync(
			gameProviderCode: Arg.Is(gameProviderCode),
			playerCode: Arg.Is(playerCode),
			gameId: Arg.Is(gameId),
			loginRecordId: Arg.Is(loginRecordId.ToString()),
			language: Arg.Is(language),
			platformType: Arg.Is(platformType),
			ipAddress: Arg.Is(ipAddress),
			backUrl: Arg.Is(backUrl))
			.Returns(gameUrl);

		var sut = new GameUrlRepository(
			fakeGameProvidersClient);

		var actual = await sut.GetGameUrlAsync(
			gameProviderCode: gameProviderCode,
			playerCode: playerCode,
			gameId: gameId,
			loginRecordId: loginRecordId,
			language: language,
			platformType: platformType,
			ipAddress: ipAddress,
			backUrl: backUrl);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
