using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface ILicenseeRepository
    {
        ValueTask<LicenseeStatus> GetStatusAsync(Guid licenseeId);
    }
}
