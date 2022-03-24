using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        /// Gets the specified hosting environment.
        /// </summary>
        /// <param name="_hostingEnvironment">The hosting environment.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get([FromServices] IWebHostEnvironment _hostingEnvironment)
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);
            var clientIp = Request.HttpContext.Connection.RemoteIpAddress;
            var hostName = Dns.GetHostName();
            var hostEntry = await Dns.GetHostEntryAsync(hostName);
            var hostIp = Array.Find(hostEntry.AddressList,
                x => x.AddressFamily == AddressFamily.InterNetwork);
            return new
            {
                ClientIp = clientIp?.ToString() ?? string.Empty,
                UA = clientInfo.UA,
                OS = clientInfo.OS,
                Device = clientInfo.Device,
                Data = new
                {
                    Environment.MachineName,
                    HostName = hostName,
                    HostIp = hostIp?.ToString() ?? string.Empty,
                    Environment = _hostingEnvironment.EnvironmentName,
                    OsVersion = $"{Environment.OSVersion}",
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                    ProcessCount = Environment.ProcessorCount
                }
            };
        }
    }
}
