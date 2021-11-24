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
        private readonly TgsContext _tgsContext;

        public GameProviderRepository(TgsContext tgsContext)
        {
            _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
        }

        public ValueTask<GameProviderModel?> GetAsync(Guid gameProviderId)
            => _tgsContext.GameProviders
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
            .FirstOrDefaultAsync();

        public ValueTask<GameProviderModel?> GetAsync(string gameProviderCode)
            => _tgsContext.GameProviders
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
            .FirstOrDefaultAsync();

        public ValueTask<GameProviderStatus> GetStatusAsync(Guid gameProviderId) => throw new NotImplementedException();

        public ValueTask<SubGameProviderModel> GetSubGameProvider(string subGameProviderCode) => throw new NotImplementedException();
    }
}
