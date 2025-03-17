using System.Text.Json;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace API.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly IDatabase _redisCache;
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/YOUR_ACCOUNT_ID/YOUR_QUEUE_NAME";

    public TransactionController(TransactionService transactionService, IConnectionMultiplexer redis, IAmazonSQS sqsClient)
    {
        _transactionService = transactionService;
        _redisCache = redis.GetDatabase();
        _sqsClient = sqsClient;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetDailyBalance(DateTime date)
    {
        string cacheKey = $"balance:{date.ToShortDateString()}";
        string cachedBalance = await _redisCache.StringGetAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedBalance))
        {
            return Ok(JsonSerializer.Deserialize<object>(cachedBalance));
        }

        var balance = await _transactionService.GetDailyBalanceAsync(date);

        if (balance != null)
        {
            await _redisCache.StringSetAsync(cacheKey, JsonSerializer.Serialize(new { date = date.ToShortDateString(), balance }), TimeSpan.FromHours(1));
        }

        var messageBody = JsonSerializer.Serialize(new { date = date.ToShortDateString(), balance, requestTime = DateTime.UtcNow });
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };
        await _sqsClient.SendMessageAsync(sendMessageRequest);

        return Ok(new { date = date.ToShortDateString(), balance });
    }

    [HttpPost]
    public async Task<IActionResult> AddTransaction(decimal amount, string type)
    {
        string cacheKey = $"transaction:{amount}:{type}";
        string cachedTransaction = await _redisCache.StringGetAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedTransaction))
        {
            return Ok(JsonSerializer.Deserialize<object>(cachedTransaction));
        }

        try
        {
            await _transactionService.AddTransactionAsync(amount, type);
            var response = new { message = "Transaction added successfully" };

            await _redisCache.StringSetAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(10));

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
