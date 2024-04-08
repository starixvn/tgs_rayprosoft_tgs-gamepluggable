namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record PlayerBalanceQuery(
	string PlayerCode,
	string GameProviderCode);
