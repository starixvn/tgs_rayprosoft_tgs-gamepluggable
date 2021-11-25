using System;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
