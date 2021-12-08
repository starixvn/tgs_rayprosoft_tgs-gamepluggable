using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        private readonly TgsContext _context;

        public PlayerRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<PlayerModel> GetAsync(string playerCode)
            => (from player in _context.Players.AsQueryable()
                join brand in _context.Brands.AsQueryable()
                on player.BrandId equals brand.Id
                join licensee in _context.Licensees.AsQueryable()
                on brand.LicenseeId equals licensee.Id
                join reseller in _context.Resellers.AsQueryable()
                on licensee.ResellerId equals reseller.Id
                where player.PlayerCode == playerCode
                select new PlayerModel(
                    reseller.Id,
                    reseller.Code,
                    licensee.Id,
                    licensee.Code,
                    brand.Id,
                    brand.Code,
                    player.PlayerCode,
                    player.Username,
                    player.RealName,
                    player.DefaultWalletId,
                    player.DefaultWalletCurrencyCode,
                    (PlayerStatus)player.Status
                    ))
            .AsAsyncEnumerable()
            .FirstAsync();
    }
}
