using System;

namespace Tgs.GameProviders.Pluggable.Core.Models
{
    public record BrandGameProviderInfo(
        Guid GameProviderId,
        Guid GameProviderConfigurationId,
        string DefaultLanguageCode,
        string DesktopQueryString,
        string MobileQueryString);
}
