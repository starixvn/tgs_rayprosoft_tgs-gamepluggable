namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record PlayerBalanceResult(
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
