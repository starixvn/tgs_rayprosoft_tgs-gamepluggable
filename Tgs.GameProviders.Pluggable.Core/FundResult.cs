using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record FundResult(
         Guid TransactionsId,
         string GameProviderTransactionId);
}
