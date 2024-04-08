using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Tgs.GameFundTransfer.Client;
using Tgs.GamePluggable.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
	private static readonly ILogger _Logger = ApplicationLogging.CreateLogger("ServiceRegisterExtensions");

	public static async Task<WebApplication> UseGameProviderRegisterAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var gameProviderRepository = scope.ServiceProvider.GetRequiredService<IGameProviderRepository>();
			var gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

			var gameProviderCodes = await gameProviderRepository.QueryAsync()
				.ToArrayAsync()
				.ConfigureAwait(false)
				?? Enumerable.Empty<string>();

			foreach (var gameProviderCode in gameProviderCodes)
			{
				try
				{
					var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
						code: gameProviderCode)
						.ConfigureAwait(false);

					_ = await gameProviderRepository
						.GetAsync(
						id: gameProviderId)
						.ConfigureAwait(false);
					try
					{
						var gameIds = await gameRepository.QueryAsync(
							gameProviderId)
							.ToArrayAsync()
							.ConfigureAwait(false)
							?? Enumerable.Empty<int>();

						foreach (var gameId in gameIds)
						{
							try
							{
								_ = await gameRepository.GetAsync(
									id: gameId)
									.ConfigureAwait(false);
							}
							catch (Exception ex)
							{
								_Logger.LogError(ex, "LogOn:{logOn} Mesaage:An exception occurred while caching this game.({gameId})",
									DateTime.Now.ToString("o"),
									gameId);
							}
						}
					}
					catch (Exception ex)
					{
						_Logger.LogError(ex, "LogOn:{logOn} Mesaage:An exception occurred while caching this game query.({gameProviderId})",
							DateTime.Now.ToString("o"),
							gameProviderId);
					}
				}
				catch (Exception ex)
				{
					_Logger.LogError(ex, "LogOn:{logOn} Mesaage:An exception occurred while caching this game provider.({gameProviderCode})",
						DateTime.Now.ToString("o"),
						gameProviderCode);
				}
			}
		}

		return app;
	}
}
