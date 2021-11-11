using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record FundResult
    {
        public Guid TransactionsId { get; init; }
        public string GameProviderTransactionId { get; init; } = default!;
    }
}
