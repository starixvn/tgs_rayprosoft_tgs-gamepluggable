using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderConfigModel
    {
        public Guid Id { get; set; }
        public Guid GameProviderId { get; set; }
        public short Status { get; set; }
        public string SecretKey { get; set; } = default!;
        public bool IsDefault { get; set; }
        public string ConfigurationData { get; set; } = default!;
        public string DesktopEndpoint { get; set; } = default!;
        public string MobileEndpoint { get; set; } = default!;
    }
}