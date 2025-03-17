using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMongoCollection<Transaction> _transactions;

        public TransactionRepository(IMongoDatabase database)
        {
            _transactions = database.GetCollection<Transaction>("transactions");
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _transactions.InsertOneAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetDailyTransactionsAsync(DateTime date)
        {
            return await _transactions.Find(t => t.CreatedAt.Date == date.Date).ToListAsync();
        }
    }
}
