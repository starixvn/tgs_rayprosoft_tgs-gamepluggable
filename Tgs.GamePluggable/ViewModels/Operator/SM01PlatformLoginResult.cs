namespace Tgs.GamePluggable.ViewModels.Operator;

public record SM01PlatformLoginResult(
	string ApiUrl,
	string MerchantId,
	string CustomerName,
	long Timestamp,
	string Sign);
