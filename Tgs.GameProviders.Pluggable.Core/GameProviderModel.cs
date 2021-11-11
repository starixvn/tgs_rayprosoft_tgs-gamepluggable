using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderModel
    {
        public Guid GameProviderId { get; set; }
        public GameProviderStatus Status { get; set; }
        public bool AutoCloseRound { get; set; }
        public bool SupportTestPlayer { get; set; }
        public bool AllowKickPlayer { get; set; }
        public short PlaceBetTimeout { get; set; }
        public short SettleBetTimeout { get; set; }
        public int UniqueId { get; set; }
        public bool TransferWallet { get; set; }
        public string GameProviderCode { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
