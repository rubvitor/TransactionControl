using Amazon.SQS.Model;
using Amazon.SQS;
using Domain.Entities;
using System.Text.Json;

namespace Application.Services;

public static class SQSManager
{
    private const int MaxMessages = 1;
    private const int WaitTime = 2;
    public static async Task StartAsync(string queue, TransactionService transactionService)
    {
        if (string.IsNullOrEmpty(queue))
        {
            Console.WriteLine("\nUsage: SQSReceiveMessages queue_url");
            Console.WriteLine("   queue_url - The URL of an existing SQS queue.");
            return;
        }
        if (!queue.StartsWith("https://sqs."))
        {
            return;
        }

        var sqsClient = new AmazonSQSClient();

        Console.WriteLine($"Reading messages from queue\n  {queue}");
        Console.WriteLine("Press any key to stop. (Response might be slightly delayed.)");
        do
        {
            var msg = await GetMessage(sqsClient, queue, WaitTime);
            if (msg.Messages.Count != 0)
            {
                if (await ProcessMessageAsync(msg.Messages[0], transactionService))
                    await DeleteMessage(sqsClient, msg.Messages[0], queue);
            }
        } while (!Console.KeyAvailable);
    }

    private static async Task<ReceiveMessageResponse> GetMessage(
      IAmazonSQS sqsClient, string qUrl, int waitTime = 0)
    {
        return await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = qUrl,
            MaxNumberOfMessages = MaxMessages,
            WaitTimeSeconds = waitTime
        });
    }

    private static async Task<bool> ProcessMessageAsync(Message message, TransactionService transactionService)
    {
        Console.WriteLine($"\nMessage body of {message.MessageId}:");
        Console.WriteLine($"{message.Body}");

        var transactionQueueMessage = JsonSerializer.Deserialize<TransactionQueueMessage>(message.Body);

        await transactionService.AddTransactionAsync(transactionQueueMessage.Amount, transactionQueueMessage.Type);

        return true;
    }

    private static async Task DeleteMessage(
      IAmazonSQS sqsClient, Message message, string qUrl)
    {
        Console.WriteLine($"\nDeleting message {message.MessageId} from queue...");
        await sqsClient.DeleteMessageAsync(qUrl, message.ReceiptHandle);
    }
}
