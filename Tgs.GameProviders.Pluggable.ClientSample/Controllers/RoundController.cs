using System;
using System.Collections.Generic;
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
        /// Games the action query asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /e8a90f53-11e7-40ab-9ed3-08d99f799e5a/GameActionQuery
        /// </remarks>
        [HttpGet("{roundId}/GameActionQuery")]
        public IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(
            [FromServices] IGameProvidersPluggableClient client,
            Guid roundId)
            => client.GameActionQueryAsync(roundId);

        /// <summary>
        /// Games the round closed asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Put /e8a90f53-11e7-40ab-9ed3-08d99f799e5a/ClosedGameRound
        ///             {
        ///                 "betAmount": 20,
        ///                 "validBetAmount": 20,
        ///                 "settleAmount": 0,
        ///                 "commission": 0,
        ///                 "turnover": 20,
        ///                 "winLoss": -20,
        ///                 "clientRoundStatus": 4,
        ///                 "externalClosedOn": "2021-11-10T14:41:24.654+08:00",
        ///                 "operatorId": "ff73bf6f-637e-4575-3cfd-08d9262ce1a5",
        ///                 "remark": "測試關閉"
        ///             }
        /// </remarks>
        [HttpPut("{roundId}/GameRoundClosed")]
        public ValueTask GameRoundClosedAsync(
            [FromServices] IGameProvidersPluggableClient client,
            Guid roundId,
            [FromBody] GameRoundClosedViewModel viewModel)
            => client.GameRoundClosedAsync(roundId, viewModel);

        /// <summary>
        /// Games the round get asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get /e8a90f53-11e7-40ab-9ed3-08d99f799e5a
        /// </remarks>
        [HttpGet("{roundId}")]
        public ValueTask<GameRoundViewModel?> GameRoundGetAsync(
            [FromServices] IGameProvidersPluggableClient client,
            Guid roundId)
            => client.GameRoundGetAsync(roundId);
    }
}
