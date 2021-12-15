using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderModel(
        Guid GameProviderId,
        GameProviderStatus Status,
        string? Description,
        bool AutoCloseRound,
        bool SupportTestPlayer,
        bool AllowKickPlayer,
        short PlaceBetTimeout,
        short SettleBetTimeout,
        int UniqueId,
        bool TransferWallet,
        string GameProviderCode,
        string Name,
        string? Localization
        );
}
