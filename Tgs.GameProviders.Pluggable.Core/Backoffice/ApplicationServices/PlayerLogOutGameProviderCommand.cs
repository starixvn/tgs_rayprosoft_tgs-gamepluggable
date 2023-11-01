namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

public record PlayerLogOutGameProviderCommand(
    string PlayerCode,
    string GameProviderCode);
