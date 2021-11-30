namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    public record FundOutQuery(
        string PlayerCode,
        string GameProviderCode,
        decimal Amount);
}
