using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class LicenseeRepository : ILicenseeRepository
    {
        public ValueTask<LicenseeStatus> GetStatusAsync(Guid licenseeId) => throw new NotImplementedException();
    }
}
