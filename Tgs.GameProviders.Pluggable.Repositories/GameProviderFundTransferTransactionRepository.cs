using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameProviderFundTransferTransactionRepository : IGameProviderFundTransferTransactionRepository
    {
        private readonly TgsContext _context;

        public GameProviderFundTransferTransactionRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async ValueTask<Guid> AddFundInAsync(
            Guid gameProviderId,
            Guid WalletId,
            string playerCode,
            string gameProviderCode,
            decimal amount,
            string description)
        {
            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            var systemDateTime = DateTime.UtcNow;
            var fundTransferTransaction = new GameProviderFundTransferTransaction()
            {
                GameProviderFundTransferTransactionId = gameProviderFundTransferTransactionId,
                GameProviderId = gameProviderId,
                WalletId = WalletId,
                PlayerCode = playerCode,
                FundSource = (short)FundSource.FundIn,
                FromWallet = gameProviderCode,
                ToWallet = GameProviderType.MainWallet,
                Amount = amount,
                Description = description,
                Status = (short)FundTransferTransactionStatus.FailureOnGameProvider,
                CreatedOn = systemDateTime,
                UpdatedOn = systemDateTime
            };
            _ = await _context.GameProviderFundTransferTransactions
                .AddAsync(fundTransferTransaction)
                .ConfigureAwait(false);

            _ = await _context
                 .SaveChangesAsync()
                 .ConfigureAwait(false);

            return gameProviderFundTransferTransactionId;
        }

        public async ValueTask<Guid> AddFundOutAsync(
            Guid gameProviderId,
            Guid WalletId,
            string playerCode,
            string gameProviderCode,
            decimal amount,
            string description)
        {
            var gameProviderFundTransferTransactionId = Guid.NewGuid();
            var systemDateTime = DateTime.UtcNow;
            var fundTransferTransaction = new GameProviderFundTransferTransaction()
            {
                GameProviderFundTransferTransactionId = gameProviderFundTransferTransactionId,
                GameProviderId = gameProviderId,
                WalletId = WalletId,
                PlayerCode = playerCode,
                FundSource = (short)FundSource.FundOut,
                FromWallet = GameProviderType.MainWallet,
                ToWallet = gameProviderCode,
                Amount = amount,
                Description = description,
                Status = (short)FundTransferTransactionStatus.FailureOnMainWallet,
                CreatedOn = systemDateTime,
                UpdatedOn = systemDateTime
            };
            _ = await _context.GameProviderFundTransferTransactions
                .AddAsync(fundTransferTransaction)
                .ConfigureAwait(false);

            _ = await _context
                  .SaveChangesAsync()
                  .ConfigureAwait(false);

            return gameProviderFundTransferTransactionId;
        }

        public async ValueTask UpdateByFailureOnGameProviderAsync(
            Guid gameProviderFundTransferTransactionId,
            Guid? transactionsId,
            string gameProviderTransactionId,
            decimal postBalance,
            string description)
        {
            var fundTransferTransaction = await _context.GameProviderFundTransferTransactions
                            .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                            .FirstAsync()
                            .ConfigureAwait(false);

            fundTransferTransaction.TransactionId = transactionsId;
            fundTransferTransaction.GameProviderTransactionId = gameProviderTransactionId;
            fundTransferTransaction.Status = (short)FundTransferTransactionStatus.FailureOnGameProvider;
            fundTransferTransaction.Balance = postBalance;
            fundTransferTransaction.UpdatedOn = DateTime.UtcNow;
            fundTransferTransaction.Description = description;

            _ = _context.GameProviderFundTransferTransactions
               .Update(fundTransferTransaction);

            _ = await _context
                 .SaveChangesAsync()
                 .ConfigureAwait(false);
        }

        public async ValueTask UpdateByFailureOnMainWalletAsync(
                    Guid gameProviderFundTransferTransactionId,
            Guid? transactionsId,
            string gameProviderTransactionId,
            decimal postBalance,
            string description)
        {
            var fundTransferTransaction = await _context.GameProviderFundTransferTransactions
                            .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                            .FirstAsync()
                            .ConfigureAwait(false);

            fundTransferTransaction.TransactionId = transactionsId;
            fundTransferTransaction.GameProviderTransactionId = gameProviderTransactionId;
            fundTransferTransaction.Status = (short)FundTransferTransactionStatus.FailureOnMainWallet;
            fundTransferTransaction.Balance = postBalance;
            fundTransferTransaction.UpdatedOn = DateTime.UtcNow;
            fundTransferTransaction.Description = description;

            _ = _context.GameProviderFundTransferTransactions
               .Update(fundTransferTransaction);

            _ = await _context
                 .SaveChangesAsync()
                 .ConfigureAwait(false);
        }

        public async ValueTask UpdateBySuccessAsync(
            Guid gameProviderFundTransferTransactionId,
            Guid? transactionsId,
            string gameProviderTransactionId,
            decimal postBalance,
            string description)
        {
            var fundTransferTransaction = await _context.GameProviderFundTransferTransactions
                            .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                            .FirstAsync()
                            .ConfigureAwait(false);

            fundTransferTransaction.TransactionId = transactionsId;
            fundTransferTransaction.GameProviderTransactionId = gameProviderTransactionId;
            fundTransferTransaction.Status = (short)FundTransferTransactionStatus.Success;
            fundTransferTransaction.Balance = postBalance;
            fundTransferTransaction.UpdatedOn = DateTime.UtcNow;
            fundTransferTransaction.Description = description;

            _ = _context.GameProviderFundTransferTransactions
               .Update(fundTransferTransaction);

            _ = await _context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }
}
