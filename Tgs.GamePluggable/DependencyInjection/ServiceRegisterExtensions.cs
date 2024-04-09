using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Tgs.GamePluggable.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
	public static async Task<WebApplication> UseGamePluggableRegisterAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(i => i.ColorBehavior = LoggerColorBehavior.Disabled));

			var logger = loggerFactory.CreateLogger<Program>();

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

					/*
					logger.LogInformation("LogOn:{logOn} Mesaage:This game provider performs caching.({gameProviderCode})",
						DateTime.Now.ToString("o"),
						gameProviderCode);
					*/
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
								/*
								logger.LogInformation("LogOn:{logOn} Mesaage:This game is cached({gameId})",
									DateTime.Now.ToString("o"),
									gameId);
								*/
								_ = await gameRepository.GetAsync(
									id: gameId)
									.ConfigureAwait(false);
							}
							catch (Exception ex)
							{
								logger.LogError(ex, "LogOn:{logOn} Mesaage:An exception occurred while caching this game.({gameId})",
									DateTime.Now.ToString("o"),
									gameId);
							}
						}
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "LogOn:{logOn} Mesaage:An exception occurred while caching this game query.({gameProviderId})",
							DateTime.Now.ToString("o"),
							gameProviderId);
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Mesaage:An exception occurred while caching this game provider.({gameProviderCode})",
						DateTime.Now.ToString("o"),
						gameProviderCode);
				}
			}
		}

		return app;
	}
}
