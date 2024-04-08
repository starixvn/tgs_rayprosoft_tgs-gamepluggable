using System;

namespace Tgs.GamePluggable.Core.Models;
public record GameProviderConfigInfo(
	Guid GameProviderId,
	string GameProviderCode,
	Guid GameProviderConfigId,
	Enums.GameProviderConfigStatus Status,
	string Name,
	string? Description,
	string ClientId,
	string SecretKey,
	string DesktopEndpoint,
	string MobileEndpoint,
	string ConfigurationData,
	bool IsDefault);
