using Xunit;
using Moq;
using Application.Services;
using Domain.Interfaces;
using Domain.Entities;
using System.Threading.Tasks;

public class TransactionServiceTests
{
    [Fact]
    public async Task AddTransactionAsync_ValidData_ShouldCallRepository()
    {
        var repositoryMock = new Mock<ITransactionRepository>();
        var service = new TransactionService(repositoryMock.Object);

        await service.AddTransactionAsync(100, "credit");

        repositoryMock.Verify(r => r.AddTransactionAsync(It.IsAny<Transaction>()), Times.Once);
    }
}
