using FCG.Catalog.Application.Consumers;
using FCG.Catalog.WebApi.Settings.Dtos;
using FCG.Core.Objects;
using MassTransit;
using Microsoft.Extensions.Options;
using System.Security.Authentication;

namespace FCG.Catalog.WebApi.Settings
{
    public static class MassTransitSettings
    {
        public static WebApplicationBuilder AddMassTransitSettings(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentProcessedEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitSettings = context
                        .GetRequiredService<IOptions<RabbitMqSettings>>()
                        .Value;

                    cfg.Host(rabbitSettings.Host, 5671, "/", h =>
                    {
                        h.Username(rabbitSettings.Username);
                        h.Password(rabbitSettings.Password);
                        h.UseSsl(s =>
                        {
                            s.Protocol = SslProtocols.Tls12;

                            s.ServerName = rabbitSettings.Host; // O ServerName deve ser igual ao Host para validação do certificado SSL
                        });
                    });

                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(
                            RetrySettings.MaxRetryAttempts,
                            TimeSpan.FromSeconds(RetrySettings.DelayBetweenRetriesInSeconds)
                        );
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return builder;
        }
    }
}
