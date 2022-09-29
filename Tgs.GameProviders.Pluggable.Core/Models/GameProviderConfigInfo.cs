using System;

namespace Tgs.GameProviders.Pluggable.Core.Models;

public record GameProviderConfigInfo(
    Guid GameProviderConfigId,
    Guid GameProviderId,
    string DesktopEndpoint,
    string MobileEndpoint,
    string ConfigurationData);
