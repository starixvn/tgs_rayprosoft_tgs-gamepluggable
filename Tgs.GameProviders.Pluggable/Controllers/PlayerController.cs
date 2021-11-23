using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Delete kickplayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<KickPlayerCommand> commandService,
            string playerCode)
            => commandService.ExecuteAsync(new KickPlayerCommand(playerCode));

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Delete kickplayer/4ucmqplxb029/ky
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}/{gameProviderCode}")]
        public ValueTask KickPlayerGameProviderAsync(
            [FromServices] ICommandService<KickPlayerGameProviderCommand> commandService,
            string playerCode,
            string gameProviderCode)
            => commandService.ExecuteAsync(new(playerCode, gameProviderCode));
    }
}
