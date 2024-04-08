using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Backoffice.Players;

public class KickPlayerUnitTest
{
	[Fact]
	public async Task KickPlayerAsync()
	{
		var playerCode = "4ucmqplxb029";

		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<ICommandService<PlayerLogOutCommand>>();

				var command = new PlayerLogOutCommand(
					playerCode);

				_ = fakeService
				.ExecuteAsync(
					Arg.Is(command),
					Arg.Any<CancellationToken>());

				_ = services.AddSingleton(fakeService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersBackofficePluggableClient(httpClient);

		await sut.KickPlayerAsync(playerCode);
	}
}
