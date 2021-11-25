namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    public record PlayerBalanceQuery(
         string GameProviderCode,
         string PlayerCode);
}
