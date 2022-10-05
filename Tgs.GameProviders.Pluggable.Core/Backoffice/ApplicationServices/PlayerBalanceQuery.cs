namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

public record PlayerBalanceQuery(
    string PlayerCode,
    string GameProviderCode);
