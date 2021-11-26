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
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now

            };
            await _context.GameProviderFundTransferTransactions
                .AddAsync(fundTransferTransaction)
                .ConfigureAwait(false);

            await _context
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
                Status = (short)FundTransferTransactionStatus.FailureOnSystem,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };
            await _context.GameProviderFundTransferTransactions
                .AddAsync(fundTransferTransaction)
                .ConfigureAwait(false);

            await _context
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return gameProviderFundTransferTransactionId;
        }

        public async ValueTask UpdateFundInAsync(
            Guid gameProviderFundTransferTransactionId,
            Guid? transactionsId,
            FundTransferTransactionStatus fundTransferTransactionStatus,
            string gameProviderTransactionId,
            decimal postBalance,
            string description)
        {
            var fundTransferTransaction = await _context.GameProviderFundTransferTransactions
                 .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                 .Select(x => new GameProviderFundTransferTransaction()
                 {
                     GameProviderFundTransferTransactionId = x.GameProviderFundTransferTransactionId,
                     TransactionId = transactionsId,
                     GameProviderId = x.GameProviderId,
                     WalletId = x.WalletId,
                     GameProviderTransactionId = gameProviderTransactionId,
                     PlayerCode = x.PlayerCode,
                     FundSource = x.FundSource,
                     FromWallet = x.FromWallet,
                     ToWallet = x.ToWallet,
                     Status = (short)fundTransferTransactionStatus,
                     Amount = x.Amount,
                     Balance = postBalance,
                     CreatedOn = x.CreatedOn,
                     UpdatedOn = DateTime.Now,
                     Description = description
                 })
                 .FirstAsync()
                 .ConfigureAwait(false);

            _context.GameProviderFundTransferTransactions
               .Update(fundTransferTransaction);

            await _context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        public async ValueTask UpdateFundOutAsync(
            Guid gameProviderFundTransferTransactionId,
            Guid? transactionsId,
            FundTransferTransactionStatus fundTransferTransactionStatus,
            string gameProviderTransactionId,
            decimal postBalance,
            string description)
        {
            var fundTransferTransaction = await _context.GameProviderFundTransferTransactions
                 .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                 .Select(x => new GameProviderFundTransferTransaction()
                 {
                     GameProviderFundTransferTransactionId = x.GameProviderFundTransferTransactionId,
                     TransactionId = transactionsId,
                     GameProviderId = x.GameProviderId,
                     WalletId = x.WalletId,
                     GameProviderTransactionId = gameProviderTransactionId,
                     PlayerCode = x.PlayerCode,
                     FundSource = x.FundSource,
                     FromWallet = x.FromWallet,
                     ToWallet = x.ToWallet,
                     Status = (short)fundTransferTransactionStatus,
                     Amount = x.Amount,
                     Balance = postBalance,
                     CreatedOn = x.CreatedOn,
                     UpdatedOn = DateTime.Now,
                     Description = description
                 })
                 .FirstAsync()
                 .ConfigureAwait(false);

            _context.GameProviderFundTransferTransactions
               .Update(fundTransferTransaction);

            await _context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }
}
