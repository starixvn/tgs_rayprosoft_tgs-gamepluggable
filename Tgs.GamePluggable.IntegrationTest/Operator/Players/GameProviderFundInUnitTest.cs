using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Operator.Players;

public class GameProviderFundInUnitTest
{
	[Fact]
	public async Task GameProviderFundInAsync()
	{
		string gameProviderCode = GameProviderType.Kaiyuan;
		string playerCode = "4ucmqplxb029";
		var amount = 1m;

		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			  {
				  var fakeFundInCommandService = Substitute.For<ICommandService<FundInCommand>>();

				  var command = new FundInCommand(
					  playerCode,
					  gameProviderCode,
					  amount);
				  _ = fakeFundInCommandService
				  .ExecuteAsync(Arg.Is(command));

				  _ = services.AddSingleton(fakeFundInCommandService);
			  }));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersOperatorPluggableClient(httpClient);

		await sut.GameProviderFundInAsync(
			playerCode: playerCode,
			gameProviderCode: gameProviderCode,
			amount: amount);
	}
}
