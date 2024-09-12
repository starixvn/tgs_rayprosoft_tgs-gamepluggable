using System;

namespace Tgs.GamePluggable.Core.Models;
public record GameInfo(
	int GameId,
	string GameCode,
	Guid GameProviderId,
	string GameProviderCode,
	string ExternalGameId,
	Enums.GameType Type,
	Enums.GamePlatformType PlatformType,
	string? EndpointPath,
	string? RuleUrl,
	string[]? AvailableCurrencies,
	string SubGameProviderCode);
