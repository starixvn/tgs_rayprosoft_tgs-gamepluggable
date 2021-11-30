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
    internal class BrandRepository : IBrandRepository
    {
        private readonly TgsContext _context;

        public BrandRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAsyncEnumerable<string> GetCurrenciesAsync(Guid brandId)
            => _context.BrandCurrencies
            .Where(x => x.BrandId == brandId)
            .Select(x => x.CurrencyCode)
            .AsAsyncEnumerable();

        public ValueTask<string?> GetCurrencyAsync(Guid brandId, string currencyCode)
            => _context.BrandCurrencies
            .Where(x => x.BrandId == brandId
                     && x.CurrencyCode == currencyCode)
            .Select(x => x.CurrencyCode)
            .AsAsyncEnumerable()
            .FirstOrDefaultAsync();

        public ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId)
            => (from brand in _context.Brands.AsQueryable()
                join brandGameProvider in _context.BrandGameProviders.AsQueryable()
                on brand.Id equals brandGameProvider.BrandId
                where
                brand.Id == brandId &&
                brandGameProvider.GameProviderId == gameProviderId
                select (GameProviderStatus)brandGameProvider.Status
               )
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId)
            => (from brand in _context.Brands.AsQueryable()
                join brandGame in _context.BrandGames.AsQueryable()
                on brand.Id equals brandGame.BrandId
                where
                brand.Id == brandId &&
                brandGame.GameId == gameId
                select (GameStatus)brandGame.Status
               )
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<BrandStatus> GetStatusAsync(Guid brandId)
            => _context.Brands
            .Where(x => x.Id == brandId)
            .Select(x => (BrandStatus)x.Status)
            .ToAsyncEnumerable()
            .FirstAsync();
    }
}
