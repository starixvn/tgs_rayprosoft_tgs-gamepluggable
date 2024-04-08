using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class GameRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var gameId = 1;

		var game = new GameInfo(
			GameId: 1,
			GameCode: "lobby",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.Kaiyuan,
			GameExternalId: "lobby",
			Type: GameType.CardGameLobby,
			PlatformType: GamePlatformType.DesktopAndMobile,
			Name: GameProviderType.Kaiyuan,
			SubGame: true,
			AvailableToBrand: true,
			HasTurnoverOnTieBet: true,
			Html5: true,
			Description: GameProviderType.Kaiyuan,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null,
			Localization: null,
			SubGameProviderCode: GameProviderType.Kaiyuan);
		_ = fakeClient
			.GameGetAsync(
				gameId: Arg.Is(gameId))
			.Returns(game);

		var repository = new GameRepository(
			fakeClient);

		var actual = await repository.GetAsync(gameId);

		Assert.NotNull(actual);
	}
}
