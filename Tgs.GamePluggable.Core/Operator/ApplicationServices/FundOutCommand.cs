namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

public record FundOutCommand(
	string PlayerCode,
	string GameProviderCode,
	decimal Amount);
