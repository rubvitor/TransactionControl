using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTransaction(decimal amount, string type)
    {
        try
        {
            await _transactionService.AddTransactionAsync(amount, type);
            return Ok(new { message = "Transaction added successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetDailyBalance(DateTime date)
    {
        var balance = await _transactionService.GetDailyBalanceAsync(date);
        return Ok(new { date = date.ToShortDateString(), balance });
    }
}
