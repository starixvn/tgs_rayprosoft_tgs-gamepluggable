using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameProviderFundTransferTransactionRepository
    {
        ValueTask<Guid> AddFundInAsync(Guid gameProviderId, Guid WalletId, string playerCode, string gameProviderCode, decimal amount, string description);

        ValueTask<Guid> AddFundOutAsync(Guid gameProviderId, Guid WalletId, string playerCode, string gameProviderCode, decimal amount, string description);

        ValueTask UpdateFundInAsync(Guid gameProviderFundTransferTransactionId, Guid? transactionsId, FundTransferTransactionStatus fundTransferTransactionStatus, string gameProviderTransactionId, decimal postBalance, string description);

        ValueTask UpdateFundOutAsync(Guid gameProviderFundTransferTransactionId, Guid? transactionsId, FundTransferTransactionStatus fundTransferTransactionStatus, string gameProviderTransactionId, decimal postBalance, string description);
    }
}
