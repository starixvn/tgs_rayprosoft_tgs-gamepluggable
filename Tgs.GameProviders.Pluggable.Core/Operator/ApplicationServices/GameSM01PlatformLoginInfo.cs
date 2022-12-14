namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

public record GameSM01PlatformLoginInfo(
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
