using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        public ValueTask<PlayerModel> GetAsync(string playerCode) => throw new NotImplementedException();
    }
}
