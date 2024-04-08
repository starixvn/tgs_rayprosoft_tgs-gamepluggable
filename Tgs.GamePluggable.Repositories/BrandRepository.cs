using Microsoft.EntityFrameworkCore;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories;

internal class BrandRepository(
	TgsContext context) : IBrandRepository
{
	public async ValueTask<bool> GetCurrencyExistAsync(Guid brandId, string currencyCode, CancellationToken cancellationToken = default)
		=> await context.BrandCurrencies.AnyAsync(
			predicate: brandCurrency => brandCurrency.BrandId == brandId &&
			brandCurrency.CurrencyCode == currencyCode,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default)
		=> await context.BrandGameProviders
		.Where(bgp => bgp.GameProviderId == gameProviderId && bgp.BrandId == brandId)
		.Select(bgp => (Guid?)bgp.GameProviderConfigurationId)
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(
			paramName: new { BrandId = brandId, GameProviderId = gameProviderId }.ToString(),
			message: $"Then brand game provider configuration doesn't exist.({brandId}|{gameProviderId})");

	public async ValueTask<bool> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default)
		=> await context.BrandGameProviders
		.Where(
			brandGameProvider => brandGameProvider.BrandId == brandId && brandGameProvider.GameProviderId == gameProviderId)
		.Join(
			context.GameProviders,
			brandGameProvider => brandGameProvider.GameProviderId,
			gameProvider => gameProvider.Id,
			(brandGameProvider, gameProvider) => new
			{
				GameProviderId = gameProvider.Id,
				GameProviderStatus = gameProvider.Status,
				brandGameProvider.BrandId,
				BrandGameProviderStatus = brandGameProvider.Status
			})
		.AnyAsync(
			predicate: brandGameProvider => brandGameProvider.GameProviderStatus == 0 && brandGameProvider.BrandGameProviderStatus == 0,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

	public async ValueTask<bool> GetGameStatusAsync(Guid brandId, int gameId, CancellationToken cancellationToken = default)
		=> await context.BrandGames
		.Where(
			bg => bg.BrandId == brandId && bg.GameId == gameId)
		.Join(
			context.Games,
			brandGame => brandGame.GameId,
			game => game.Id,
			(brandGame, game) => new
			{
				game.GameProviderId,
				GameId = game.Id,
				GameStatus = game.Status,
				brandGame.BrandId,
				BrandGameStatus = brandGame.Status
			})
		.Join(
			context.BrandGameProviders,
			conform => conform.GameProviderId,
			brandGameProvider => brandGameProvider.GameProviderId,
			(conform, brandGameProvider) => new
			{
				conform.GameProviderId,
				conform.GameId,
				conform.GameStatus,
				conform.BrandId,
				BrandGameProviderStatus = brandGameProvider.Status,
				conform.BrandGameStatus,
			})
		.Join(
			context.GameProviders,
			conform => conform.GameProviderId,
			gameProvider => gameProvider.Id,
			(conform, gameProvider) => new
			{
				conform.GameProviderId,
				GameProviderStatus = gameProvider.Status,
				conform.GameId,
				conform.GameStatus,
				conform.BrandId,
				conform.BrandGameProviderStatus,
				conform.BrandGameStatus,
			})
		.AnyAsync(
			predicate: brandGame => brandGame.GameProviderStatus == 0 &&
			brandGame.GameStatus == 0 &&
			brandGame.BrandGameProviderStatus == 0 &&
			brandGame.BrandGameStatus == 0,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
