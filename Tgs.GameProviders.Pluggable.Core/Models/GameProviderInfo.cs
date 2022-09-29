using System;

namespace Tgs.GameProviders.Pluggable.Core.Models
{
    public record GameProviderInfo(
        Guid GameProviderId,
        string GameProviderCode);
}
