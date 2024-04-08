using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
	public static async Task<WebApplication> InitialConnectionGameFundTransferAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var client = scope.ServiceProvider.GetRequiredService<IGameFundTransferClient>();
			_ = await client.CheckingServiceAsync(
				machineName: Environment.MachineName)
				.ConfigureAwait(false);
		}

		return app;
	}

	public static async Task<WebApplication> UseGameProviderRegisterAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();
			var gameProvider = scope.ServiceProvider.GetRequiredService<IGameProvider>();
			var game = scope.ServiceProvider.GetRequiredService<IGame>();

			var gameProviderInfos = await client
				.GameProviderQueryAsync()
				.ToArrayAsync()
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameProviderInfo>();

			foreach (var gameProviderInfo in gameProviderInfos)
			{
				_ = await gameProvider
					.GetAsync(
					gameProviderId: gameProviderInfo.GameProviderId)
					.ConfigureAwait(false);
				_ = await gameProvider
					.GetGameProviderIdAsync(
					gameProviderCode: gameProviderInfo.GameProviderCode)
					.ConfigureAwait(false);

				try
				{
					var gameInfos = await client
						.GameQueryAsync(
						gameProviderInfo.GameProviderCode)
						.ToArrayAsync()
						.ConfigureAwait(false)
						?? Enumerable.Empty<GameInfo>();

					foreach (var gameInfo in gameInfos)
					{
						_ = await game
							.GetAsync(
							gameInfo.GameId)
							.ConfigureAwait(false);
					}
				}
				catch
				{
				}
			}
		}

		return app;
	}
}
