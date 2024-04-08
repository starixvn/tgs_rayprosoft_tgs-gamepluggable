namespace Tgs.GameProviders.Pluggable.ViewModels;

public record PlayerBalanceInfoViewModel(
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
