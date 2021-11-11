using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameProviderFundTransferTransactionRepository : IGameProviderFundTransferTransactionRepository
    {
        public ValueTask<Guid> AddFundInAsync(Guid gameProviderId, Guid WalletId, string playerCode, string gameProviderCode, decimal amount, string description) => throw new NotImplementedException();

        public ValueTask<Guid> AddFundOutAsync(Guid gameProviderId, Guid WalletId, string playerCode, string gameProviderCode, decimal amount, string description) => throw new NotImplementedException();

        public ValueTask UpdateFundInAsync(Guid gameProviderFundTransferTransactionId, Guid? transactionsId, FundTransferTransactionStatus fundTransferTransactionStatus, string gameProviderTransactionId, decimal postBalance, string description) => throw new NotImplementedException();

        public ValueTask UpdateFundOutAsync(Guid gameProviderFundTransferTransactionId, Guid? transactionsId, FundTransferTransactionStatus fundTransferTransactionStatus, string gameProviderTransactionId, decimal postBalance, string description) => throw new NotImplementedException();
    }
}
