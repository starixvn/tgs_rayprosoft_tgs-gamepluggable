using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class LicenseeRepository : ILicenseeRepository
    {
        private readonly TgsContext _context;

        public LicenseeRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<LicenseeStatus> GetStatusAsync(Guid licenseeId)
        => _context.Licensees
            .Where(x => x.Id == licenseeId)
            .Select(x => (LicenseeStatus)x.Status)
            .ToAsyncEnumerable().FirstAsync();
    }
}
