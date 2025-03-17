using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task AddTransactionAsync(decimal amount, string type)
        {
            if (amount <= 0 || (type != "credit" && type != "debit"))
                throw new ArgumentException("Invalid transaction data");

            var transaction = new Transaction(amount, type);
            await _transactionRepository.AddTransactionAsync(transaction);
        }

        public async Task<decimal> GetDailyBalanceAsync(DateTime date)
        {
            var transactions = await _transactionRepository.GetDailyTransactionsAsync(date);
            return transactions.Sum(t => t.Type == "credit" ? t.Amount : -t.Amount);
        }
    }
}
