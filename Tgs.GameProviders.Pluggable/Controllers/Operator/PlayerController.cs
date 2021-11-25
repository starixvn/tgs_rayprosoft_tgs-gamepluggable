using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
namespace Tgs.GameProviders.Pluggable.Controllers.Operator
{
    [Route("op/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {

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
            ) => service.GetAsync(new PlayerBalanceQuery(gameProviderCode, playerCode));
    }
}
