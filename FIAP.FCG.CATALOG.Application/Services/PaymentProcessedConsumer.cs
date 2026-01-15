using FIAP.FCG.CATALOG.Core.Inputs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace FIAP.FCG.CATALOG.Application.Services
{
    // *****************************************************************
    // classe consumidora do rabbitMQ
    // monitora a fila PaymentProcessedEvent e grava no banco adicionando o jogo à biblioteca do usuário
    // *****************************************************************
    public class PaymentProcessedConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentProcessedConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            Console.WriteLine("=== MESSAGE CONSUMER ===");
            Console.WriteLine("Conectando ao RabbitMQ...");

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin123"
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                string queueName = "PaymentProcessedEvent";

                // Declara a fila (criar se não existir)
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                Console.WriteLine($"Aguardando mensagens da fila '{queueName}'...");
                Console.WriteLine("Pressione CTRL+C para sair.");

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        // Desserializa para o objeto
                        var paymentProcessed = JsonSerializer.Deserialize<PaymentProcessedDto>(json);

                        if (paymentProcessed == null)
                        {
                            Console.WriteLine("Mensagem recebida, mas não foi possível desserializar.");
                            return;
                        }

                        Console.WriteLine("====================================");
                        Console.WriteLine("📦 Processamento de pagamento recebido:");
                        Console.WriteLine($"🆔 OrderId.........: {paymentProcessed.orderId}");
                        Console.WriteLine($"👤 PaymentStatus...: {paymentProcessed.status}");
                        Console.WriteLine("====================================");

                        
                        using var scope = _scopeFactory.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                        var order = await orderService.GetById(paymentProcessed.orderId);                        

                        OrderUpdateDto orderUpdate = new OrderUpdateDto();

                        string status = "";

                        switch (paymentProcessed.status)
                        {
                            case 1:
                                status = "Authorized";
                                break;
                            case 2:
                                status = "Paid";
                                break; 
                            case 3:
                                status = "Decline";
                                break;
                            case 4:
                                status = "Refunded";
                                break;
                            case 5:
                                status = "Cancelled";
                                break;
                            default: // Executado se nenhuma das opções acima corresponder
                                status = "Opção Inválida"; ;
                                break;
                        }

                        orderUpdate.OrderDate = order.OrderDate;
                        orderUpdate.UserId = order.UserId;
                        orderUpdate.GameId = order.GameId;
                        orderUpdate.Price = order.Price;
                        orderUpdate.PaymentStatus = status; // pega o paymentStatus que veio na mensagem do rabbitMQ
                        orderUpdate.CardName = order.CardName;
                        orderUpdate.CardNumber = order.CardNumber;
                        orderUpdate.ExpirationDate = order.ExpirationDate;
                        orderUpdate.Cvv = order.Cvv;

                        await orderService.Update(paymentProcessed.orderId, orderUpdate);

                        Console.WriteLine("✅ Retorno do pagamento processado com sucesso!\n");

                        // ✅ CONFIRMA para o RabbitMQ
                        await channel.BasicAckAsync(ea.DeliveryTag, false); // chat recomendou
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("❌ Erro ao processar mensagem:");
                        Console.WriteLine(ex.Message);
                        await channel.BasicNackAsync(ea.DeliveryTag, false, true); // chat recomendou
                    }
                };


                await channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);

                // Mantém o consumer rodando
                //Console.ReadLine();
                await Task.Delay(Timeout.Infinite, stoppingToken); // recomendação do chat
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                Console.WriteLine("Certifique-se de que o RabbitMQ está rodando (execute: ./rabbitmq.sh start)");
            }
        }
    }
}
