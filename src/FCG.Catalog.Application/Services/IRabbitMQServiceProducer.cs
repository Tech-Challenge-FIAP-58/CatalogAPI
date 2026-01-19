namespace FCG.Catalog.Application.Services
{
    public interface IRabbitMQServiceProducer
    {
        Task SendMessageAsync(string message);

        Task SendMessageAsyncObjeto<T>(T message);
    }
}
