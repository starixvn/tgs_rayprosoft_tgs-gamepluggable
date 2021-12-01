using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record SubGameProviderModel
    {
        public string SubGameProviderCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid GameProviderId { get; set; }
        public string? Localization { get; set; }
    }
}
