namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record PlayerLogOutGameProviderCommand(
	string PlayerCode,
	string GameProviderCode);
