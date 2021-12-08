namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    public record FundInQuery(
        string PlayerCode,
        string GameProviderCode,
        decimal Amount);
}
