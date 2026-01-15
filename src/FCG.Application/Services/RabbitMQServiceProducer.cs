using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FCG.Application.Services
{
    public class RabbitMQServiceProducer : IRabbitMQServiceProducer
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queueName;

        public RabbitMQServiceProducer(IConfiguration configuration)
        {
            _configuration = configuration;
            _hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";
            _userName = _configuration["RabbitMQ:UserName"] ?? "guest";
            _password = _configuration["RabbitMQ:Password"] ?? "guest";
            _queueName = _configuration["RabbitMQ:QueueName"] ?? "OrderPlacedEvent";// "PaymentProcessedEvent";
        }

        public async Task SendMessageAsync(string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Declara a fila (criar se não existir)
            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Prepara a mensagem
            var messageBody = Encoding.UTF8.GetBytes(message);


            // Envia a mensagem
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                body: messageBody);
        }

        public async Task SendMessageAsyncObjeto<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serializa o objeto para JSON
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                body: body);
        }

    }
}
