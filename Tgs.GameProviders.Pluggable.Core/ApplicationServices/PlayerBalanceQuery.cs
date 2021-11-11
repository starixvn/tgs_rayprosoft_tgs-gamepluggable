namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record PlayerBalanceQuery
    {
        public string GameProviderCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;
    }
}
