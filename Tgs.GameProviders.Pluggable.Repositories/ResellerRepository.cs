using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class ResellerRepository : IResellerRepository
    {
        public ValueTask<ResellerStatus> GetStatusAsync(Guid licenseeId) => throw new NotImplementedException();
    }
}
