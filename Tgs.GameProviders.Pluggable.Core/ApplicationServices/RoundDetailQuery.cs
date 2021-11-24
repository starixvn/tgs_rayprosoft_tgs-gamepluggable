using System;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    public record RoundDetailQuery
    {
        public Guid RoundId { get; init; }

        public string Language { get; init; } = default!;
    }
}
