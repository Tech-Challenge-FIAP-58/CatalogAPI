using Amazon.SQS;
using Amazon.SQS.Model;
using FCG.Catalog.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FCG.Catalog.Application.Producers;

public class SqsNotificationProducer(IAmazonSQS sqsClient, IConfiguration configuration) : ISqsNotificationProducer
{
    private readonly string _queueUrl = configuration["SQS:QueueUrl"]!;

    public async Task Send(string destinatario, string assunto, string corpo)
    {
        var body = JsonSerializer.Serialize(new
        {
            Message = new { Destinatario = destinatario, Assunto = assunto, Corpo = corpo }
        });

        await sqsClient.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = body
        });
    }
}
