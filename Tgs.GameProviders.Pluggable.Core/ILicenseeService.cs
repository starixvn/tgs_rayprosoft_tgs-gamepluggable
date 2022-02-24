using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface ILicenseeService
    {
        ValueTask<LicenseeStatus> GetStatusAsync(Guid licenseeId);
    }
}
