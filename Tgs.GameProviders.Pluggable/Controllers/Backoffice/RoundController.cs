using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Controllers.Backoffice
{
    [Route("bo/[controller]")]
    [ApiController]
    public class RoundController : ControllerBase
    {
        [HttpPut("addCompanyRound")]
        public ValueTask AddCompanyRoundAsync(
            [FromServices] IGameRoundsClient gameRoundsClient,
            [FromBody] AddCompanyRoundViewModel viewModel)
        {
            _ = gameRoundsClient.AddCompanyRoundAsync(
                new GameRounds.Common.ViewModels.AddCompanyRoundViewModel
                {
                    BrandId = viewModel.BrandId,
                    PlayerCode = viewModel.PlayerCode,
                    WalletId = viewModel.WalletId,
                    GameId = viewModel.GameId,
                    GameProviderId = viewModel.GameProviderId,
                    GameProviderConfigurationId = viewModel.GameProviderConfigurationId,
                    ExternalRoundId = viewModel.ExternalRoundId,
                    ClientStatus = (GameRoundClientStatus)viewModel.ClientRoundStatus,
                    ExternalCreatedOn = viewModel.ExternalCreatedOn,
                    ExternalClosedOn = viewModel.ExternalClosedOn,
                    CurrencyCode = viewModel.CurrencyCode,
                    BetAmount = viewModel.BetAmount,
                    SettleAmount = viewModel.SettleAmount,
                    ValidBetAmount = viewModel.ValidBetAmount,
                    Turnover = viewModel.Turnover,
                    Commission = viewModel.Commission,
                    CompanyWinLoss = viewModel.CompanyWinLoss,
                    OperatorId = viewModel.OperatorId,
                    Remark = viewModel.Remark
                },
                HttpContext.RequestAborted);

            return ValueTask.CompletedTask;
        }

        [HttpPut("{roundId}/adjustclosedRound")]
        public ValueTask AdjustClosedGameRoundAsync(
            [FromServices] ICommandService<AdjustClosedRoundCommand> commandService,
            Guid roundId,
            [FromBody] AdjustClosedRoundViewModel viewModel)
            => commandService.ExecuteAsync(new AdjustClosedRoundCommand()
            {
                RoundId = roundId,
                BetAmount = viewModel.BetAmount,
                ValidBetAmount = viewModel.ValidBetAmount,
                SettleAmount = viewModel.SettleAmount,
                Commission = viewModel.Commission,
                Turnover = viewModel.Turnover,
                WinLoss = viewModel.WinLoss,
                RoundClientStatus = (GameRoundClientStatus)viewModel.ClientRoundStatus,
                ExternalClosedOn = viewModel.ExternalClosedOn,
                OperatorId = viewModel.OperatorId,
                Remark = viewModel.Remark,
                AdjustAmount = viewModel.AdjustAmount
            },
            HttpContext.RequestAborted);

        /// <summary>
        /// Games the action query asynchronous.
        /// </summary>
        /// <param name="queryService">The query service.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /bo/round/e8a90f53-11e7-40ab-9ed3-08d99f799e5a/GameActionQuery
        /// </remarks>
        [HttpGet("{roundId}/gameactionquery")]
        public IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(
                    [FromServices] IQueryService<Guid, GameActionModel> queryService,
                    Guid roundId)
            => queryService.QueryAsync(roundId)
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
        ///     Put /bo/round/15426ced-d81b-42a6-a0a3-08d971a93a52/ClosedGameRound
        ///     {
        ///         "betAmount": 100,
        ///         "validBetAmount": 0,
        ///         "settleAmount": 0,
        ///         "commission": 0,
        ///         "turnover": 0,
        ///         "winLoss": 0,
        ///         "clientRoundStatus": 2,
        ///         "externalClosedOn": "2021-09-07T17:40:24.000+08:00",
        ///         "operatorId": "7acf882a-474b-ec11-bf75-8c554ae8ab6b",
        ///         "remark": "測試關閉",
        ///         "adjustAmount": 100
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
                            RoundClientStatus = (GameRoundClientStatus)viewModel.ClientRoundStatus,
                            ExternalClosedOn = viewModel.ExternalClosedOn,
                            OperatorId = viewModel.OperatorId,
                            Remark = viewModel.Remark,
                            AdjustAmount = viewModel.AdjustAmount
                        });

        /// <summary>
        ///
        /// </summary>
        /// <param name="getService"></param>
        /// <param name="roundId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /bo/round/3c6282df-d531-4482-088e-08d9a004e811/gamerounddetail?language=zh-CN
        /// </remarks>
        [HttpGet("{roundId}/gamerounddetail")]
        public async ValueTask<JsonDocument> GameRoundDetailGetAsync(
            [FromServices] IGetService<RoundDetailQuery, JsonDocument> getService,
            Guid roundId,
            [FromQuery] string language)
            => await getService.GetAsync(
                new()
                {
                    RoundId = roundId,
                    Language = language
                })
                .ConfigureAwait(false);

        /// <summary>
        /// Games the round get asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /bo/round/e8a90f53-11e7-40ab-9ed3-08d99f799e5a
        /// </remarks>
        [HttpGet("{roundId}")]
        public async ValueTask<GameRoundViewModel> GameRoundGetAsync(
                    [FromServices] IGetService<Guid, GameRoundInfo> getService,
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
