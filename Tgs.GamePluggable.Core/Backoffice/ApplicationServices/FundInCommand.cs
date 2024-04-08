namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record FundInCommand(
	string PlayerCode,
	string GameProviderCode);
