namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

public record KickPlayerGameProviderCommand(
    string PlayerCode,
    string GameProviderCode);
