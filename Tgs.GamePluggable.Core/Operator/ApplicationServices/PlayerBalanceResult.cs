namespace Tgs.GamePluggable.Core.Operator.ApplicationServices;

public record PlayerBalanceResult(
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
