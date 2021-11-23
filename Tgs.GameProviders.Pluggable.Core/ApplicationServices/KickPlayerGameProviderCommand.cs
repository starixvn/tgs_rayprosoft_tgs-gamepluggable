namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record KickPlayerGameProviderCommand(
        string PlayerCode,
        string GameProviderCode);
}
