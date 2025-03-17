using Domain.Entities;

namespace Domain.Interfaces;

public interface ITransactionRepository
{
    Task AddTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetDailyTransactionsAsync(DateTime date);
}
