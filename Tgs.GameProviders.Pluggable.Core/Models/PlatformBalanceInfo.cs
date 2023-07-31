namespace Tgs.GameProviders.Pluggable.Core.Models;

public record PlatformBalanceInfo(
    decimal Balance,
    decimal FreeBalance,
    decimal OutstandingBalance);
