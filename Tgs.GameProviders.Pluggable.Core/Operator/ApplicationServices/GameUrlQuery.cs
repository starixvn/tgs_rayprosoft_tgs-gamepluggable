using System;
using Tgs.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

public record GameUrlQuery(
    string PlayerCode,
    Guid LoginRecordId,
    GameProvidersClientPlatformType PlatformType,
    int GameId,
    string Language,
    string IpAddress,
    Uri? BackUrl);
