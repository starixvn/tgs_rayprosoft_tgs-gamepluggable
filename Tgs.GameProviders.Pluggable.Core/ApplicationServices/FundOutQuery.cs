namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record FundOutQuery
    {
        public string GameProviderCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public decimal Amount { get; init; } = default!;
    }
}
