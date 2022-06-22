namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    public record FundInCommand(
        string PlayerCode,
        string GameProviderCode);
}
