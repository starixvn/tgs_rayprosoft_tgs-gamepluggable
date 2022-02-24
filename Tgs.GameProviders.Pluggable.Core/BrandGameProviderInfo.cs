using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record BrandGameProviderInfo(
        Guid GameProviderId,
        Guid GameProviderConfigurationId,
        string DefaultLanguageCode,
        string DesktopQueryString,
        string MobileQueryString);
}
