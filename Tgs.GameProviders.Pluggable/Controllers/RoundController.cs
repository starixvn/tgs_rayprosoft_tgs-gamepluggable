using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.ServiceUtility;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoundController : ControllerBase
    {
        /// <summary>
        /// Games the action query asynchronous.
        /// </summary>
        /// <param name="queryService">The query service.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /e8a90f53-11e7-40ab-9ed3-08d99f799e5a/GameActionQuery
        /// </remarks>
        [HttpGet("{roundId}/gameactionquery")]
        public IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(
                    [FromServices] IQueryService<Guid, GameActionModel> queryService,
                    Guid roundId)
            => queryService.QueryAsync(
                roundId)
            .Select(data => new GameActionViewModel(
                data.Id,
                data.RoundId,
                data.PlayerCode,
                data.ExternalTransactionId,
                (GameActionViewModelType)data.Type,
                data.Amount,
                data.Turnover,
                data.WinLoss,
                data.UnsettledBets,
                data.Balance,
                data.BetType,
                data.ExternalBetId,
                data.ExternalGameActionGroupId,
                data.ReferencedGameActionId,
                data.GameActionBatchId,
                data.RecordedOn,
                data.SentOn,
                data.IsClosingRound,
                data.ClosingRoundExplicitRequest,
                data.PlayerIp,
                data.ExtraData,
                data.JackpotContribution,
                data.StoredOn
                ));

        /// <summary>
        /// Closeds the game round asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Put /e8a90f53-11e7-40ab-9ed3-08d99f799e5a/ClosedGameRound
        ///     {
        ///         "betAmount": 20,
        ///         "validBetAmount": 20,
        ///         "settleAmount": 0,
        ///         "commission": 0,
        ///         "turnover": 20,
        ///         "winLoss": -20,
        ///         "clientRoundStatus": 4,
        ///         "externalClosedOn": "2021-11-10T14:41:24.654+08:00",
        ///         "operatorId": "ff73bf6f-637e-4575-3cfd-08d9262ce1a5",
        ///         "remark": "測試關閉"
        ///     }
        /// </remarks>
        [HttpPut("{roundId}/gameroundclosed")]
        public ValueTask GameRoundClosedAsync(
                    [FromServices] ICommandService<GameRoundClosedCommand> commandService,
                    Guid roundId,
                    [FromBody] GameRoundClosedViewModel viewModel)
                    => commandService.ExecuteAsync(new()
                    {
                        RoundId = roundId,
                        BetAmount = viewModel.BetAmount,
                        ValidBetAmount = viewModel.ValidBetAmount,
                        SettleAmount = viewModel.SettleAmount,
                        Commission = viewModel.Commission,
                        Turnover = viewModel.Turnover,
                        WinLoss = viewModel.WinLoss,
                        ClientRoundStatus = (ClientRoundStatus)viewModel.ClientRoundStatus,
                        ExternalClosedOn = viewModel.ExternalClosedOn,
                        OperatorId = viewModel.OperatorId,
                        Remark = viewModel.Remark,
                    });

        /// <summary>
        /// Games the round get asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /e8a90f53-11e7-40ab-9ed3-08d99f799e5a
        /// </remarks>
        [HttpGet("{roundId}")]
        public async ValueTask<GameRoundViewModel> GameRoundGetAsync(
                    [FromServices] IGetService<Guid, GameRoundModel> getService,
                    Guid roundId)
        {
            var data = await getService.GetAsync(
                roundId)
                .ConfigureAwait(false);
            return new GameRoundViewModel()
            {
                RoundId = data.RoundId,
                ExternalRoundId = data.ExternalRoundId,
                GameProviderId = data.GameProviderId,
                PlayerCode = data.PlayerCode,
                GameId = data.GameId,
                CurrencyCode = data.CurrencyCode,
                Status = (int)data.Status,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                WalletId = data.WalletId,
                CreatedOn = data.CreatedOn,
                ClosedOn = data.ClosedOn,
                UpdatedOn = data.UpdatedOn,
                BrandId = data.BrandId,
                ExternalCreatedOn = data.ExternalCreatedOn,
                ExternalClosedOn = data.ExternalClosedOn,
                BetAmount = data.BetAmount,
                ValidBetAmount = data.ValidBetAmount,
                SettleAmount = data.SettleAmount,
                Commission = data.Commission,
                Turnover = data.Turnover,
                WinLoss = data.WinLoss,
                TieBetAdjustment = data.TieBetAdjustment,
                ClientStatus = (int)data.ClientStatus,
                ReopenTurnoverAdjustment = data.ReopenTurnoverAdjustment,
                ReopenedOn = data.ReopenedOn,
                EffectiveBettingPercentage = data.EffectiveBettingPercentage
            };
        }
    }
}
