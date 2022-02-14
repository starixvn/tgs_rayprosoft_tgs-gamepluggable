using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

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

        /// <summary>
        /// Gets the game url asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Post /op/player/gameurl
        ///         {
        ///             "playerCode": "4ucmqplxb029",
        ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "isMobile": false,
        ///             "gameId": 1000032,
        ///             "language": "zh-CN",
        ///             "ipAddress": "127.0.0.1",
        ///             "backUrl": "https://dev-t1.rpgp.cc"
        ///         }
        /// </remarks>
        [HttpPost("gameurl")]
        public ValueTask<Uri?> GetGameUrlAsync(
            [FromServices] IGetService<GameUrlQuery, Uri?> service,
            [FromBody] GameProviderGameUrlViewModel model)
            => service
                .GetAsync(new(
                    model.PlayerCode,
                    model.LoginRecordId,
                    model.IsMobile ? GameProvidersClientPlatformType.Mobile : GameProvidersClientPlatformType.Desktop,
                    model.GameId,
                    model.Language,
                    model.IpAddress,
                    model.BackUrl));
    }
}
