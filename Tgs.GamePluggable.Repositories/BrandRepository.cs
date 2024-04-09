using System.ComponentModel;
using Grpc.Net.Client.Balancer;
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
		.Join(
			context.Brands,
			brandGameProvider => brandGameProvider.BrandId,
			brand => brand.Id,
			(brandGameProvider, brand) => new
			{
				brand.LicenseeId,
				BrandId = brand.Id,
				brandGameProvider.GameProviderId,
				BrandGameProviderStatus = brandGameProvider.Status
			})
		.Join(
			context.Licensees,
			conform => conform.LicenseeId,
			licensee => licensee.Id,
			(conform, licensee) => new
			{
				licensee.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.BrandGameProviderStatus
			})
		.Join(
			context.ResellerGameProviders,
			conform => new { conform.ResellerId, conform.GameProviderId },
			resellerGameProvider => new { resellerGameProvider.ResellerId, resellerGameProvider.GameProviderId },
			(conform, reseller) => new
			{
				conform.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.BrandGameProviderStatus
			})
		.Join(
			context.LicenseeGameProviders,
			conform => new { conform.LicenseeId, conform.GameProviderId },
			licenseeGameProvider => new { licenseeGameProvider.LicenseeId, licenseeGameProvider.GameProviderId },
			(conform, reseller) => new
			{
				conform.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.BrandGameProviderStatus
			})
		.Join(
			context.GameProviders,
			conform => conform.GameProviderId,
			gameProvider => gameProvider.Id,
			(conform, gameProvider) => new
			{
				conform.GameProviderId,
				GameProviderStatus = gameProvider.Status,
				conform.BrandId,
				conform.BrandGameProviderStatus
			})
		.AnyAsync(
			predicate: brandGameProvider => brandGameProvider.BrandId == brandId &&
			brandGameProvider.GameProviderId == gameProviderId &&
			brandGameProvider.GameProviderStatus == 0 &&
			brandGameProvider.BrandGameProviderStatus == 0,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

	public async ValueTask<bool> GetGameStatusAsync(Guid brandId, int gameId, CancellationToken cancellationToken = default)
		=> await context.BrandGames
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
			context.Brands,
			conform => conform.BrandId,
			brand => brand.Id,
			(conform, brand) => new
			{
				brand.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.GameId,
				conform.GameStatus,
				conform.BrandGameStatus
			})
		.Join(
			context.Licensees,
			conform => conform.LicenseeId,
			licensee => licensee.Id,
			(conform, licensee) => new
			{
				licensee.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.GameId,
				conform.GameStatus,
				conform.BrandGameStatus
			})
		.Join(
			context.ResellerGameProviders,
			conform => new { conform.ResellerId, conform.GameProviderId },
			resellerGameProvider => new { resellerGameProvider.ResellerId, resellerGameProvider.GameProviderId },
			(conform, reseller) => new
			{
				conform.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.GameId,
				conform.GameStatus,
				conform.BrandGameStatus
			})
		.Join(
			context.LicenseeGameProviders,
			conform => new { conform.LicenseeId, conform.GameProviderId },
			licenseeGameProvider => new { licenseeGameProvider.LicenseeId, licenseeGameProvider.GameProviderId },
			(conform, reseller) => new
			{
				conform.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.GameId,
				conform.GameStatus,
				conform.BrandGameStatus
			})
		.Join(
			context.BrandGameProviders,
			conform => new { conform.BrandId, conform.GameProviderId },
			brandGameProvider => new { brandGameProvider.BrandId, brandGameProvider.GameProviderId },
			(conform, brandGameProvider) => new
			{
				conform.ResellerId,
				conform.LicenseeId,
				conform.BrandId,
				conform.GameProviderId,
				conform.GameId,
				conform.GameStatus,
				BrandGameProviderStatus = brandGameProvider.Status,
				conform.BrandGameStatus
			})
		.Join(
			context.GameProviders,
			conform => conform.GameProviderId,
			gameProvider => gameProvider.Id,
			(conform, gameProvider) => new
			{
				GameProviderStatus = gameProvider.Status,
				conform.GameId,
				conform.GameStatus,
				conform.BrandId,
				conform.BrandGameProviderStatus,
				conform.BrandGameStatus
			})
		.AnyAsync(
			predicate: brandGame =>
			brandGame.BrandId == brandId && brandGame.GameId == gameId &&
			brandGame.GameProviderStatus == 0 &&
			brandGame.GameStatus == 0 &&
			brandGame.BrandGameProviderStatus == 0 &&
			brandGame.BrandGameStatus == 0,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
