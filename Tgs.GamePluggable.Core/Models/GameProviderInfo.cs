using System;

namespace Tgs.GamePluggable.Core.Models;
public record GameProviderInfo(
	Guid GameProviderId,
	string GameProviderCode,
	bool TransferWallet,
	bool AllowKickPlayer);
