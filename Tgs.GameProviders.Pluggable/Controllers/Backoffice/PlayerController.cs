using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels.Backoffice;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Controllers.Backoffice
{
    [Route("bo/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Post
        ///         {
        ///             "playerCode": "4ucmqplxb029",
        ///             "gameProviderCode": "ky"
        ///         }
        /// </remarks>
        [HttpPost("fund/in", Name = "Backoffice_Player_FundIn")]
        public ValueTask<bool> FundInAsync(
            [FromServices] IGetService<FundInQuery, bool> service,
              FundInQueryViewModel model)
            => service.GetAsync(new FundInQuery(
                model.PlayerCode,
                model.GameProviderCode));

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
        ///         /bo/balance/player/4ucmqplxb029/KY
        /// </remarks>
        [HttpGet("balance/{playerCode}/{gameProviderCode}", Name = "Backoffice_Player_GetBalance")]
        public ValueTask<decimal?> GetBalanceAsync(
            [FromServices] IGetService<PlayerBalanceQuery, decimal?> service,
            string playerCode,
            string gameProviderCode)
            => service.GetAsync(new PlayerBalanceQuery(gameProviderCode, playerCode));

        /// <summary>
        /// Kicks the asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Delete
        ///         /bo/player/kick/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kick/{playerCode}", Name = "Backoffice_Player_KickPlayer")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<KickPlayerCommand> commandService,
            string playerCode)
            => commandService.ExecuteAsync(new KickPlayerCommand(playerCode));

        /// <summary>
        /// Kicks the asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Delete
        ///         /bo/player/kick/4ucmqplxb029/ky
        /// </remarks>
        [HttpDelete("kick/{playerCode}/{gameProviderCode}", Name = "Backoffice_Player_Game_KickPlayer")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<KickPlayerGameProviderCommand> commandService,
            string playerCode,
            string gameProviderCode)
            => commandService.ExecuteAsync(new(playerCode, gameProviderCode));
    }
}
