using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPut("{roundId}/ClosedGameRound")]
        public ValueTask ClosedGameRoundAsync(
                    [FromServices] ICommandService<GameRoundClosedCommand> commandService,
                    Guid roundId,
                    [FromBody] ClosedGameRoundViewModel viewModel)
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
    }
}
