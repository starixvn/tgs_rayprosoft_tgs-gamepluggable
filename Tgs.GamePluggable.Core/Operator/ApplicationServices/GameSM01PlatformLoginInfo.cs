namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

public record GameSM01PlatformLoginInfo(
	string ApiUrl,
	string MerchantId,
	string CustomerName,
	long Timestamp,
	string Sign);
