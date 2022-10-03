namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

public record FundInCommand(
    string PlayerCode,
    string GameProviderCode,
    decimal Amount);
