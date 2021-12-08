using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record BrandGameProviderModel
    {
        public Guid GameProviderId { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public string DefaultLanguageCode { get; set; } = default!;

        public string DesktopQueryString { get; set; } = default!;

        public string MobileQueryString { get; set; } = default!;
    }
}