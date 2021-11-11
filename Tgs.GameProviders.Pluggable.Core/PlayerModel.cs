using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record PlayerModel
    {
        public Guid ResellerId { get; init; }
        public Guid LicenseeId { get; init; }
        public string LicenseeCode { get; init; } = default!;
        public Guid BrandId { get; init; }
        public string BrandCode { get; init; } = default!;
        public string PlayerCode { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string? RealName { get; init; }
        public Guid DefaultWalletId { get; init; }
        public string DefaultWalletCurrencyCode { get; init; } = default!;
        public PlayerStatus Status { get; set; }
    }
}
