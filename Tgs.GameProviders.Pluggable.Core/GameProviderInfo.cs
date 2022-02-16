using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderInfo(
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
