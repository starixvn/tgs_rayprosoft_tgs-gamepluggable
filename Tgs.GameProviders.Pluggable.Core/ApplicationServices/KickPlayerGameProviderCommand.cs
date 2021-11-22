using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record KickPlayerGameProviderCommand(
        string PlayerCode,
        string GameProviderCode);
}
