using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record GameUrlQuery
    {
        public string SubGameProviderCode { get; init; } = default!;
        public string PlayerCode { get; init; } = default!;
        public Guid LoginRecordId { get; init; }
        public PlatformType PlatformType { get; init; }
        public int GameId { get; init; }
        public string Language { get; init; } = default!;

        public string IpAddress { get; init; } = default!;

        public Uri BackUrl { get; init; } = default!;
    }
}
