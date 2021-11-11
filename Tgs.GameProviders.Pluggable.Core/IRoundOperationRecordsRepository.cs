using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IRoundOperationRecordsRepository
    {
        ValueTask AddAsync(Guid operatorId, Guid roundId, string oldData, string newData, string remark);
    }
}
