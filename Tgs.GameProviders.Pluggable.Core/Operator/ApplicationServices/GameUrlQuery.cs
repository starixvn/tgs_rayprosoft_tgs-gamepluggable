using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

public record GameUrlQuery(
    string PlayerCode,
    Guid LoginRecordId,
    PlatformType PlatformType,
    int GameId,
    string Language,
    string IpAddress,
    Uri? BackUrl);