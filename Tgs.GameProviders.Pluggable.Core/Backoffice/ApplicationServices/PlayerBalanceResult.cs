namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

public record PlayerBalanceResult(
    decimal Balance,
    decimal FreeBalance,
    decimal OutstandingBalance);
