using Microsoft.EntityFrameworkCore;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories;

internal class GameProviderRepository(
	TgsContext context) : IGameProviderRepository
{
	public async ValueTask<Core.Models.GameProviderInfo> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.GameProviders
		.Where(gp => gp.Id == id)
		.Select(gp => new Core.Models.GameProviderInfo(
			gp.Id,
			gp.Code,
			gp.TransferWallet,
			gp.AllowKickPlayer))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(paramName: id.ToString(), message: $"This game provider does not exist.{id}");

	public async ValueTask<Core.Models.GameProviderConfigInfo> GetConfigurationAsync(Guid gameProviderConfigId, CancellationToken cancellationToken = default)
		=> await context.GameProviderConfigs
		.Join(
			context.GameProviders,
			gameProviderConfig => gameProviderConfig.GameProviderId,
			gameProvider => gameProvider.Id,
			(gameProviderConfig, gameProvider) => new Core.Models.GameProviderConfigInfo(
				gameProvider.Id,
				gameProvider.Code,
				gameProviderConfig.Id,
				(Core.Enums.GameProviderConfigStatus)gameProviderConfig.Status,
				gameProviderConfig.Name,
				gameProviderConfig.Description,
				gameProviderConfig.ClientId,
				gameProviderConfig.SecretKey,
				gameProviderConfig.DesktopEndpoint,
				gameProviderConfig.MobileEndpoint,
				gameProviderConfig.ConfigurationData,
				gameProviderConfig.IsDefault))
		.Where(gp => gp.GameProviderConfigId == gameProviderConfigId)
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(paramName: gameProviderConfigId.ToString(), message: $"This game provider configuration does not exist.{gameProviderConfigId}");

	public async ValueTask<bool> GetCurrencyExistAsync(Guid id, string currencyCode, CancellationToken cancellationToken = default)
		=> await context.GameProviderCurrencies.AnyAsync(
			predicate: gameProviderCurrency => gameProviderCurrency.GameProviderId == id
			&& gameProviderCurrency.CurrencyCode == currencyCode,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<Guid> GetGameProviderIdAsync(string code, CancellationToken cancellationToken = default)
	{
		code = code.ToUpperInvariant().Trim();
		return await context.GameProviders
			.Where(gp => gp.Code == code)
			.Select(gp => (Guid?)gp.Id)
			.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(paramName: code, message: $"This game provider does not exist.{code}");
	}

	public IAsyncEnumerable<string> QueryAsync(CancellationToken cancellationToken = default)
		=> context.GameProviders
		.Where(gp => gp.Status != 1)
		.Select(gp => gp.Code)
		.AsAsyncEnumerable();
}
