using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Tgs.GamePluggable.Controllers;

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
	public async Task<object> GetAsync([FromServices] IWebHostEnvironment _hostingEnvironment)
	{
		var hostName = Dns.GetHostName();
		var hostEntry = await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
		var hostIp = Array.Find(hostEntry.AddressList,
			x => x.AddressFamily == AddressFamily.InterNetwork);
		return new
		{
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
