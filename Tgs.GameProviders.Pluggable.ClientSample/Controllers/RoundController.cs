using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.ClientSample.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoundController : ControllerBase
    {
        /// <summary>
        /// Closeds the game round asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        /// Put /e8a90f53-11e7-40ab-9ed3-08d99f799e5a/ClosedGameRound  \
        ///         { \
        ///             "betAmount": 20,        \
        ///             "validBetAmount": 20,   \
        ///             "settleAmount": 0,      \
        ///             "commission": 0,        \
        ///             "turnover": 20,         \
        ///             "winLoss": -20,         \
        ///             "clientRoundStatus": 4, \
        ///             "externalClosedOn": "2021-11-10T14:41:24.654+08:00",    \
        ///             "operatorId": "ff73bf6f-637e-4575-3cfd-08d9262ce1a5",   \
        ///             "remark": "測試關閉"    \
        ///          }
        /// </remarks>
        [HttpPut("{roundId}/ClosedGameRound")]
        public ValueTask ClosedGameRoundAsync(
            [FromServices] IGameProvidersPluggableClient client,
            Guid roundId,
            [FromBody] ClosedGameRoundViewModel viewModel)
            => client.ClosedGameRoundAsync(roundId, viewModel);
    }
}
