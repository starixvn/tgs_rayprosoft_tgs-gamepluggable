using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderConfigInfo(
        Guid GameProviderConfigId,
        string Name,
        string? Description,
        string ClientId,
        string SecretKey,
        GameProviderConfigStatus Status,
        Guid GameProviderId,
        string DesktopEndpoint,
        string MobileEndpoint,
        string ConfigurationData,
        bool IsDefault);
}
