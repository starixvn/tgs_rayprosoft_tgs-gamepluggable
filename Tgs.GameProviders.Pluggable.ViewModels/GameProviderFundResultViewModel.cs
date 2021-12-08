using System;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public record GameProviderFundResultViewModel(
             Guid TransactionsId,
             string GameProviderTransactionId);
}
