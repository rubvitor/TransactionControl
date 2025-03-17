
namespace Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } // "credit" or "debit"
    public DateTime CreatedAt { get; private set; }

    public Transaction(decimal amount, string type)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }
}
