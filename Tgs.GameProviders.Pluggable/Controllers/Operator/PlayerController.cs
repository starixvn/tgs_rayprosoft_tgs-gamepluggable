using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Controllers.Operator
{
    [Route("op/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// Games the provider fund in asynchronous(GameProvider -> Main Wallet).
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Post /op/player/fundin/4ucmqplxb029/ky
        ///         {
        ///             "amount": 1
        ///         }
        /// </remarks>
        [HttpPost("fundin/{playerCode}/{gameProviderCode}")]
        public async ValueTask<GameProviderFundResultViewModel?> GameProviderFundInAsync(
            [FromServices] IGetService<FundInQuery, FundResult?> service,
            string playerCode,
            string gameProviderCode,
            [FromBody] GameProviderFundViewModel model)
        {
            var result = await service.GetAsync(new(
                playerCode,
                gameProviderCode.ToUpper(),
                model.Amount)).ConfigureAwait(false);

            return result == null ? null : new(result.TransactionsId, result.GameProviderTransactionId);
        }

        /// <summary>
        /// Games the provider fund out asynchronous(Main Wallet -> GameProvider).
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Post /op/player/fundout/4ucmqplxb029/ky
        ///         {
        ///             "amount": 1
        ///         }
        /// </remarks>
        [HttpPost("fundout/{playerCode}/{gameProviderCode}")]
        public async ValueTask<GameProviderFundResultViewModel?> GameProviderFundOutAsync(
                [FromServices] IGetService<FundOutQuery, FundResult?> service,
                string playerCode,
                string gameProviderCode,
                [FromBody] GameProviderFundViewModel model)
        {
            var result = await service.GetAsync(new(
                playerCode,
                gameProviderCode.ToUpper(),
                model.Amount)).ConfigureAwait(false);

            return result == null ? null : new(result.TransactionsId, result.GameProviderTransactionId);
        }

        /// <summary>
        /// Gets the balance asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Get
        ///         /op/player/balance/4ucmqplxb029/KY
        /// </remarks>
        [HttpGet("balance/{playerCode}/{gameProviderCode}")]
        public ValueTask<decimal?> GetBalanceAsync(
            [FromServices] IGetService<PlayerBalanceQuery, decimal?> service,
            string playerCode,
            string gameProviderCode
            ) => service.GetAsync(new(
                playerCode,
                gameProviderCode.ToUpper()));
    }
}
