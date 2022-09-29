using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameRounds.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class GameRoundGetService : IGetService<Guid, GameRoundInfo>
    {
        private readonly IGameRoundsClient _client;

        public GameRoundGetService(IGameRoundsClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameRoundInfo> GetAsync(Guid query, CancellationToken cancellationToken = default)
        {
            var data = await _client
                .GetGameRoundAsync(
                    roundId: query,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"Game Round does not exist.({query})");

            return data.Data is null
                ? throw new Exception($"Game Round does not exist.({query})")
                : new GameRoundInfo(
                data.Data.RoundId,
                data.Data.ExternalRoundId,
                data.Data.GameProviderId,
                data.Data.PlayerCode,
                data.Data.GameId,
                data.Data.CurrencyCode,
                data.Data.Status,
                data.Data.GameProviderConfigurationId,
                data.Data.WalletId,
                data.Data.CreatedOn,
                data.Data.ClosedOn,
                data.Data.UpdatedOn,
                data.Data.BrandId,
                data.Data.ExternalCreatedOn,
                data.Data.ExternalClosedOn,
                data.Data.BetAmount,
                data.Data.ValidBetAmount,
                data.Data.SettleAmount,
                data.Data.Commission,
                data.Data.Turnover,
                data.Data.CompanyWinLoss,
                data.Data.TieBetAdjustment,
                data.Data.ClientStatus,
                data.Data.ReopenTurnoverAdjustment,
                data.Data.ReopenedOn,
                data.Data.EffectiveBettingPercentage);
        }
    }
}
