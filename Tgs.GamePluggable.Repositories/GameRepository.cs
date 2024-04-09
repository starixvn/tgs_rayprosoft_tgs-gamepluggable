using Microsoft.EntityFrameworkCore;

using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories;

internal class GameRepository(
	 TgsContext context) : IGameRepository
{
	public async ValueTask<Core.Models.GameInfo> GetAsync(int id, CancellationToken cancellationToken = default)
		=> await context.Games
		.Where(x => x.Id == id)
		.Join(context.GameProviders,
			game => game.GameProviderId,
			gameProvider => gameProvider.Id,
			(game, gameProvider) => new Core.Models.GameInfo(
				game.Id,
				game.Code,
				gameProvider.Id,
				gameProvider.Code,
				game.ExternalId,
				(Core.Enums.GameType)game.Type,
				(Core.Enums.GamePlatformType)game.PlatformType,
				game.EndpointPath,
				game.RuleUrl,
				string.IsNullOrEmpty(game.AvailableCurrencies) ? null : game.AvailableCurrencies.Split(",", StringSplitOptions.RemoveEmptyEntries))
			)
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(id.ToString(), $"This game does not exist.{id}");

	public IAsyncEnumerable<int> QueryAsync(Guid gameProviderId, CancellationToken cancellationToken = default)
		=> context.Games
		.Where(g => g.GameProviderId == gameProviderId && g.Status == 0)
		.Select(g => g.Id)
		.AsAsyncEnumerable();
}
