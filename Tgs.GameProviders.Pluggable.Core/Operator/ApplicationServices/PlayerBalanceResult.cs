namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

public record PlayerBalanceResult(
    decimal Balance,
    decimal FreeBalance,
    decimal OutstandingBalance);
