using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class ResellerRepository : IResellerRepository
    {
        private readonly TgsContext _context;

        public ResellerRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<ResellerStatus> GetStatusAsync(Guid licenseeId)
            => _context.Resellers
            .Where(x => x.Id == licenseeId)
            .Select(x => (ResellerStatus)x.Status)
            .ToAsyncEnumerable()
            .FirstAsync();
    }
}
