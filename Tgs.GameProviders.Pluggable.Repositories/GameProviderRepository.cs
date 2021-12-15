using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly TgsContext _context;

        public GameProviderRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<GameProviderModel> GetAsync(Guid gameProviderId)
            => _context.GameProviders
            .Where(x => x.Id == gameProviderId)
            .Select(data => new GameProviderModel(
                data.Id,
                (GameProviderStatus)data.Status,
                data.Description,
                data.AutoCloseRound,
                data.SupportTestPlayer,
                data.AllowKickPlayer,
                data.PlaceBetTimeout,
                data.SettleBetTimeout,
                data.UniqueId,
                data.TransferWallet,
                data.Code,
                data.Name,
                data.Localization))
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<GameProviderModel> GetAsync(string gameProviderCode)
            => _context.GameProviders
            .Where(x => x.Code == gameProviderCode)
            .Select(data => new GameProviderModel(
                data.Id,
                (GameProviderStatus)data.Status,
                data.Description,
                data.AutoCloseRound,
                data.SupportTestPlayer,
                data.AllowKickPlayer,
                data.PlaceBetTimeout,
                data.SettleBetTimeout,
                data.UniqueId,
                data.TransferWallet,
                data.Code,
                data.Name,
                data.Localization))
            .ToAsyncEnumerable()
            .FirstAsync();

        public IAsyncEnumerable<GameProviderCurrencyModel> GetCurrenciesAsync(Guid gameProviderId)
            => _context.GameProviderCurrencies
            .Where(x => x.GameProviderId == gameProviderId)
            .Select(x => new GameProviderCurrencyModel(
                x.GameProviderId,
                x.CurrencyCode,
                x.GameProviderCurrencyCode
                ))
            .AsAsyncEnumerable();

        public IAsyncEnumerable<GameProviderCurrencyModel> GetCurrenciesAsync(string gameProviderCode)
            => _context.GameProviders
            .Where(x => x.Code == gameProviderCode)
            .Join(_context.GameProviderCurrencies
                , gameProviders => gameProviders.Id
                , gameProviderCurrencies => gameProviderCurrencies.GameProviderId
                , (gameProviders, gameProviderCurrencies) => new GameProviderCurrencyModel(
                    gameProviderCurrencies.GameProviderId,
                    gameProviderCurrencies.CurrencyCode,
                    gameProviderCurrencies.GameProviderCurrencyCode
                 ))
            .AsAsyncEnumerable();

        public ValueTask<GameProviderCurrencyModel?> GetCurrencyAsync(Guid gameProviderId, string currencyCode)
            => _context.GameProviderCurrencies
            .Where(x => x.GameProviderId == gameProviderId
                    && x.CurrencyCode == currencyCode)
            .Select(x => new GameProviderCurrencyModel(
                x.GameProviderId,
                x.CurrencyCode,
                x.GameProviderCurrencyCode
                ))
            .AsAsyncEnumerable()
            .FirstOrDefaultAsync();

        public ValueTask<GameProviderCurrencyModel?> GetCurrencyAsync(string gameProviderCode, string currencyCode)
            => _context.GameProviders

            .Join(_context.GameProviderCurrencies.AsQueryable()
                , gameProviders => gameProviders.Id
                , gameProviderCurrency => gameProviderCurrency.GameProviderId
                , (gameProvider, gameProviderCurrency) => new
                {
                    GameProvider = gameProvider,
                    GameProviderCurrency = gameProviderCurrency
                })
            .Where(x => x.GameProvider.Code == gameProviderCode)
            .Where(x => x.GameProviderCurrency.CurrencyCode == currencyCode)
            .Select(x => new GameProviderCurrencyModel(
                    x.GameProviderCurrency.GameProviderId,
                    x.GameProviderCurrency.CurrencyCode,
                    x.GameProviderCurrency.GameProviderCurrencyCode))
            .AsAsyncEnumerable()
            .FirstOrDefaultAsync();

        public ValueTask<GameProviderStatus> GetStatusAsync(Guid gameProviderId)
            => _context.GameProviders
            .Where(x => x.Id == gameProviderId)
            .Select(data => (GameProviderStatus)data.Status)
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<SubGameProviderModel> GetSubGameProviderAsync(string subGameProviderCode)
            => _context.SubGameProviders
            .Where(x => x.Code == subGameProviderCode)
            .Select(data => new SubGameProviderModel()
            {
                SubGameProviderCode = data.Code,
                Name = data.Name,
                Description = data.Description,
                GameProviderId = data.GameProviderId,
                Localization = data.Localization
            })
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<GameProviderConfigModel> GetGameProviderConfigurationAsync(Guid gameProviderId, Guid gameProviderConfigurationId)
            => _context.GameProviderConfigs
            .Where(x => x.GameProviderId == gameProviderId && x.Id == gameProviderConfigurationId)
            .Select(data => new GameProviderConfigModel()
            {
                Id = data.Id,
                GameProviderId = data.GameProviderId,
                Status = data.Status,
                SecretKey = data.SecretKey,
                IsDefault = data.IsDefault,
                ConfigurationData = data.ConfigurationData,
                DesktopEndpoint = data.DesktopEndpoint,
                MobileEndpoint = data.MobileEndpoint
            })
            .ToAsyncEnumerable()
            .FirstAsync();
    }
}