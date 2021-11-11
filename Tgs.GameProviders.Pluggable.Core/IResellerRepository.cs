using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IResellerRepository
    {
        ValueTask<ResellerStatus> GetStatusAsync(Guid licenseeId);
    }
}
