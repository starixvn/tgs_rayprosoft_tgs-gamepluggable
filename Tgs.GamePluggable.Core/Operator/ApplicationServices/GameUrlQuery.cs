using System;

namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

public record GameUrlQuery(
	string PlayerCode,
	Guid LoginRecordId,
	GameProviders.Domains.PlatformType PlatformType,
	int GameId,
	string Language,
	string IpAddress,
	Uri? BackUrl,
	object? ExtraInfo);
