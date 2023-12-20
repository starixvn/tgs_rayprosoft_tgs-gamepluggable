using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Pluggable.ViewModels.Operator;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Controllers.Operator;

[Route("op/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    /// <summary>
    /// Games the provider fund in asynchronous.(GameProvider -> Main Wallet)
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="gameProviderCode">The game provider code.</param>
    /// <param name="source">The source.</param>
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
    public ValueTask GameProviderFundInAsync(
        [FromServices] ICommandService<FundInCommand> service,
        string playerCode,
        string gameProviderCode,
        [FromBody] GameProviderFundViewModel source)
    {
        var query = new FundInCommand(
            PlayerCode: playerCode,
            GameProviderCode: gameProviderCode,
            Amount: source.Amount);

        return service
            .ExecuteAsync(
            query,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Games the provider fund out asynchronous.(Main Wallet -> GameProvider)
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="gameProviderCode">The game provider code.</param>
    /// <param name="source">The source.</param>
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
    public ValueTask GameProviderFundOutAsync(
        [FromServices] ICommandService<FundOutCommand> service,
        string playerCode,
        string gameProviderCode,
        [FromBody] GameProviderFundViewModel source)
    {
        var query = new FundOutCommand(
            PlayerCode: playerCode,
            GameProviderCode: gameProviderCode,
            Amount: source.Amount);

        return service
            .ExecuteAsync(
            query,
            cancellationToken: HttpContext.RequestAborted);
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
    ///     GetAsync
    ///         /op/player/balance/4ucmqplxb029/KY
    /// </remarks>
    [HttpGet("balance/{playerCode}/{gameProviderCode}")]
    public async ValueTask<decimal> GetBalanceAsync(
        [FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
        string playerCode,
        string gameProviderCode)
    {
        var query = new PlayerBalanceQuery(
            PlayerCode: playerCode,
            GameProviderCode: gameProviderCode);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return result.FreeBalance;
    }

    /// <summary>
    /// Gets the game URL asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
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
    public ValueTask<Uri> GetGameUrlAsync(
        [FromServices] IGetService<GameUrlQuery, Uri> service,
        [FromBody] GameProviderGameUrlViewModel source)
    {
        var query = new GameUrlQuery(
            PlayerCode: source.PlayerCode,
            LoginRecordId: source.LoginRecordId,
            PlatformType: source.IsMobile ? GameProvidersClientPlatformType.Mobile : GameProvidersClientPlatformType.Desktop,
            GameId: source.GameId,
            Language: source.Language,
            IpAddress: source.IpAddress,
            BackUrl: source.BackUrl);

        return service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// ses the M01 login asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    [HttpPost("gameurl/sm01")]
    public async ValueTask<SM01PlatformLoginResult> SM01LoginAsync(
        [FromServices] IGetService<GameSM01PlatformLoginQuery, GameSM01PlatformLoginInfo> service,
        [FromBody] GameSM01LoginViewModel source)
    {
        var query = new GameSM01PlatformLoginQuery(
            PlayerCode: source.PlayerCode);

        var info = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new SM01PlatformLoginResult(info.ApiUrl, info.MerchantId, info.CustomerName, info.Timestamp, info.Sign);
    }
}
