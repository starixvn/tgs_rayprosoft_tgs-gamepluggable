using System;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record GameUrlResult
    {
        public Uri Url { get; init; } = default!;
    }
}
