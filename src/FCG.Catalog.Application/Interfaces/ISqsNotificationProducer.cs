namespace FCG.Catalog.Application.Interfaces;

public interface ISqsNotificationProducer
{
    Task Send(string destinatario, string assunto, string corpo);
}
