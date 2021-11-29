using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAParser;

namespace Tgs.GameProviders.Pluggable.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Get()
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);
            var clientIp = Request.HttpContext.Connection.RemoteIpAddress;
            return new
            {
                ClientIp = clientIp?.ToString() ?? string.Empty,
                UA = clientInfo.UA,
                OS = clientInfo.OS,
                Device = clientInfo.Device
            };
        }
    }
}
